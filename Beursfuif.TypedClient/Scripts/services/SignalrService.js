var beursfuif;
(function (beursfuif) {
    var SignalRMethodNames = (function () {
        function SignalRMethodNames() {
        }
        SignalRMethodNames.LOGIN = "logOn";
        SignalRMethodNames.ACK_TIME_UPDATE = "ackTimeUpdate";
        SignalRMethodNames.NEW_ORDER = "newOrder";
        SignalRMethodNames.ACK_INTERVAL_UPDATE = "ackIntervalUpdate";

        SignalRMethodNames.SEND_INITIAL_DATA = "sendInitialData";
        SignalRMethodNames.UPDATE_TIME = "updateTime";
        SignalRMethodNames.YOU_GOT_KICKED = "youGotKicked";
        SignalRMethodNames.ACK_NEW_ORDER = "ackNewOrder";
        SignalRMethodNames.UPDATE_INTERVAL = "updateInterval";
        SignalRMethodNames.DRINK_AVAILABLE_CHANGED = "drinkAvailableChanged";
        return SignalRMethodNames;
    })();
    beursfuif.SignalRMethodNames = SignalRMethodNames;

    var SignalrService = (function () {
        //#endregion
        function SignalrService($q, $log, $rootScope) {
            this.$q = $q;
            this.$log = $log;
            this.$rootScope = $rootScope;
        }
        SignalrService.prototype.initialize = function (url, name) {
            var _this = this;
            this.connection = $.hubConnection(url);
            this.hub = this.connection.createHubProxy("beursfuif");
            $.connection.hub.logging = true;

            this.registerCallback();

            this.connection.error(function () {
                _this.connection.stop(false, false);
                _this.unregisterCallbacks();
                _this.$rootScope.$broadcast(beursfuif.EventNames.OPEN_MODAL, beursfuif.ModalMessages.CONNECTION_LOST_TITLE, beursfuif.ModalMessages.CONNECTION_LOST);
            });

            this.connection.start(function () {
                _this.$log.info("start");
                _this.hub.invoke(SignalRMethodNames.LOGIN, name);
            }).fail(function (e) {
                console.log("fail", e);
                _this.$rootScope.$broadcast(beursfuif.EventNames.OPEN_MODAL, beursfuif.ModalMessages.CONNECTION_LOST_TITLE, beursfuif.ModalMessages.CONNECTION_LOST);
            });
        };

        //#region callbacks from the server
        SignalrService.prototype.unregisterCallbacks = function () {
            var _this = this;
            this.hub.off(SignalRMethodNames.SEND_INITIAL_DATA, function (msg) {
                _this.$log.info("Off initial data");
            });
            this.hub.off(SignalRMethodNames.UPDATE_TIME, function (msg) {
                _this.$log.info("Off update time");
            });
            this.hub.off(SignalRMethodNames.YOU_GOT_KICKED, function (msg) {
                _this.$log.info("Off you got kicked");
            });
            this.hub.off(SignalRMethodNames.ACK_NEW_ORDER, function (msg) {
                _this.$log.info("Off ack new order");
            });
            this.hub.off(SignalRMethodNames.UPDATE_INTERVAL, function (msg) {
                _this.$log.info("Off update interval");
            });
            this.hub.off(SignalRMethodNames.DRINK_AVAILABLE_CHANGED, function (msg) {
                _this.$log.info("Off drink available");
            });
        };

        SignalrService.prototype.registerCallback = function () {
            var _this = this;
            this.hub.on(SignalRMethodNames.SEND_INITIAL_DATA, function (currentTime, clientInterval) {
                return _this.sendInitialData(currentTime, clientInterval);
            });
            this.hub.on(SignalRMethodNames.UPDATE_TIME, function (currentTime, authenticationCode) {
                return _this.updateTime(currentTime, authenticationCode);
            });
            this.hub.on(SignalRMethodNames.YOU_GOT_KICKED, function () {
                return _this.kicked();
            });
            this.hub.on(SignalRMethodNames.ACK_NEW_ORDER, function () {
                return _this.showToast();
            });
            this.hub.on(SignalRMethodNames.UPDATE_INTERVAL, function (clientInterval, currentBFTime) {
                return _this.updateInterval(clientInterval, currentBFTime);
            });
            this.hub.on(SignalRMethodNames.DRINK_AVAILABLE_CHANGED, function (clientInterval) {
                return _this.drinkAvailableChanged(clientInterval);
            });
        };

        SignalrService.prototype.sendInitialData = function (currentTime, clientInterval) {
            console.log("initial data", currentTime, clientInterval);
            this.currentTime = currentTime;
            this.clientInterval = clientInterval;
            this.clientInterval.ClientDrinks.sort(this.sortByLowerDrinkName);
            this.$log.log(this.currentTime);
            this.$log.log(this.clientInterval);
            this.$rootScope.$broadcast(beursfuif.EventNames.CONNECTION_CHANGED, true);
        };

        SignalrService.prototype.updateTime = function (currentTime, authenticationCode) {
            this.$log.log("time time time", currentTime);

            this.validateAuthString(authenticationCode);

            this.currentTime = currentTime;
            this.$rootScope.$broadcast(beursfuif.EventNames.TIME_CHANGED);
        };

        SignalrService.prototype.kicked = function () {
            this.hub.connection.stop(false, true);
            this.$rootScope.$broadcast(beursfuif.EventNames.OPEN_MODAL, beursfuif.ModalMessages.YOU_GOT_KICKED_TITLE, beursfuif.ModalMessages.YOU_GOT_KICKED);

            this.hub = null;
            this.connection = null;
        };

        SignalrService.prototype.showToast = function () {
            toastr.success("Je bestelling werd goed ontvangen.", "Bestelling gelukt!");
        };

        SignalrService.prototype.updateInterval = function (clientInterval, currentBFTime) {
            this.$log.log(clientInterval, currentBFTime);
            this.currentTime = currentBFTime;
            this.clientInterval = clientInterval;
            this.clientInterval.ClientDrinks.sort(this.sortByLowerDrinkName);
            this.$rootScope.$broadcast(beursfuif.EventNames.INTERVAL_UPDATE);
            toastr.info("De prijzen werden aangepast", "Prijzen update.");

            //respond to server
            this.hub.invoke(SignalRMethodNames.ACK_INTERVAL_UPDATE, this.generateAuthString());
        };

        SignalrService.prototype.drinkAvailableChanged = function (clientInterval) {
            console.log(clientInterval);
            this.currentTime = clientInterval.CurrentTime;
            this.clientInterval = clientInterval;
            this.clientInterval.ClientDrinks.sort(this.sortByLowerDrinkName);
            this.$rootScope.$broadcast(beursfuif.EventNames.DRINK_AVAILABLE_CHANGED);
            toastr.info("Het aantal beschikbare dranken is veranderd", "Drank beschikbaarheid aangepast.");
        };

        //#endregion
        //#region Authcode
        SignalrService.prototype.validateAuthString = function (serverAuthString) {
            var clientAuthString = this.generateAuthString();
            if (clientAuthString === serverAuthString) {
                this.$log.log("Auth code mathces");

                //respond to the incoming message
                this.hub.invoke(SignalRMethodNames.ACK_TIME_UPDATE, clientAuthString);
            } else {
                this.$log.warn("Wrong auth code");
                this.connection.stop(false, true);
                this.$rootScope.$broadcast(beursfuif.EventNames.OPEN_MODAL, beursfuif.ModalMessages.WRONG_AUTH_TITLE, beursfuif.ModalMessages.WRONG_AUTH);
            }
        };

        SignalrService.prototype.generateAuthString = function () {
            var length = this.clientInterval.ClientDrinks.length;
            var clientAuth = "(" + this.clientInterval.Id + ")";
            for (var i = 0; i < length; i++) {
                clientAuth = clientAuth + "[" + this.clientInterval.ClientDrinks[i].DrinkId + ":" + this.clientInterval.ClientDrinks[i].Price + "]";
            }
            return clientAuth;
        };

        SignalrService.prototype.sortByLowerDrinkName = function (a, b) {
            if (a.Name.toLowerCase() > b.Name.toLowerCase()) {
                return 1;
            } else if (a.Name.toLowerCase() < b.Name.toLowerCase()) {
                return -1;
            }
            return 0;
        };

        //#endregion
        //#region Send to the server
        SignalrService.prototype.sendNewOrder = function (order) {
            this.hub.invoke(SignalRMethodNames.NEW_ORDER, order, this.generateAuthString());
        };
        return SignalrService;
    })();
    beursfuif.SignalrService = SignalrService;

    if (beursfuif.beamerModule) {
        beursfuif.beamerModule.factory("SignalrService", [
            "$q", "$log", "$rootScope", function ($q, $log, $rootScope) {
                return new SignalrService($q, $log, $rootScope);
            }]);
    }

    if (beursfuif.beursfuifModule) {
        beursfuif.beursfuifModule.factory("SignalrService", [
            "$q", "$log", "$rootScope", function ($q, $log, $rootScope) {
                return new SignalrService($q, $log, $rootScope);
            }]);
    }
})(beursfuif || (beursfuif = {}));
//# sourceMappingURL=SignalrService.js.map
