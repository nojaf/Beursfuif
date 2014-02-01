/// <reference path="../app/_references.ts" />
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
        return SignalRMethodNames;
    })();

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
                _this.$rootScope.$broadcast(beursfuif.EventNames.OPEN_MODAL, beursfuif.ModalMessages.CONNECTION_LOST_TITLE, beursfuif.ModalMessages.CONNECTION_LOST);
            });

            this.connection.start(function () {
                console.log("start");
                _this.hub.invoke(SignalRMethodNames.LOGIN, name);
            }).fail(function () {
                console.log("fail");
                _this.$rootScope.$broadcast(beursfuif.EventNames.OPEN_MODAL, beursfuif.ModalMessages.CONNECTION_LOST_TITLE, beursfuif.ModalMessages.CONNECTION_LOST);
            });
        };

        //#region callbacks from the server
        SignalrService.prototype.registerCallback = function () {
            var _this = this;
            this.hub.on(SignalRMethodNames.SEND_INITIAL_DATA, function () {
                var msg = [];
                for (var _i = 0; _i < (arguments.length - 0); _i++) {
                    msg[_i] = arguments[_i + 0];
                }
                return _this.sendInitialData(msg);
            });
            this.hub.on(SignalRMethodNames.UPDATE_TIME, function () {
                var msg = [];
                for (var _i = 0; _i < (arguments.length - 0); _i++) {
                    msg[_i] = arguments[_i + 0];
                }
                return _this.updateTime(msg);
            });
            this.hub.on(SignalRMethodNames.YOU_GOT_KICKED, function () {
                return _this.kicked();
            });
            this.hub.on(SignalRMethodNames.ACK_NEW_ORDER, function () {
                return _this.showToast();
            });
            this.hub.on(SignalRMethodNames.UPDATE_INTERVAL, function () {
                var msg = [];
                for (var _i = 0; _i < (arguments.length - 0); _i++) {
                    msg[_i] = arguments[_i + 0];
                }
                return _this.updateInterval(msg);
            });
        };

        SignalrService.prototype.sendInitialData = function () {
            var msg = [];
            for (var _i = 0; _i < (arguments.length - 0); _i++) {
                msg[_i] = arguments[_i + 0];
            }
            this.currentTime = msg[0][0];
            this.clientInterval = msg[0][1];
            this.clientInterval.ClientDrinks.sort(this.sortByLowerDrinkName);
            this.$log.log(this.currentTime);
            this.$log.log(this.clientInterval);
            this.$rootScope.$broadcast(beursfuif.EventNames.CONNECTION_CHANGED, true);
        };

        SignalrService.prototype.updateTime = function () {
            var msg = [];
            for (var _i = 0; _i < (arguments.length - 0); _i++) {
                msg[_i] = arguments[_i + 0];
            }
            this.$log.log("time time time");
            this.$log.log(msg);

            this.validateAuthString(msg[0][1]);

            this.currentTime = msg[0][0];
            this.$rootScope.$broadcast(beursfuif.EventNames.TIME_CHANGED);
        };

        SignalrService.prototype.kicked = function () {
            this.connection.stop(false, true);
            this.$rootScope.$broadcast(beursfuif.EventNames.OPEN_MODAL, beursfuif.ModalMessages.YOU_GOT_KICKED_TITLE, beursfuif.ModalMessages.YOU_GOT_KICKED);
        };

        SignalrService.prototype.showToast = function () {
            toastr.success("Je bestelling werd goed ontvangen.", "Bestelling gelukt!");
        };

        SignalrService.prototype.updateInterval = function () {
            var msg = [];
            for (var _i = 0; _i < (arguments.length - 0); _i++) {
                msg[_i] = arguments[_i + 0];
            }
            this.$log.log(msg);
            this.currentTime = msg[0][1];
            this.clientInterval = msg[0][0];
            this.clientInterval.ClientDrinks.sort(this.sortByLowerDrinkName);
            this.$rootScope.$broadcast(beursfuif.EventNames.INTERVAL_UPDATE);
            toastr.info("De prijzen werden aangepast", "Prijzen update.");

            //respond to server
            this.hub.invoke(SignalRMethodNames.ACK_INTERVAL_UPDATE, this.generateAuthString());
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
})(beursfuif || (beursfuif = {}));
//# sourceMappingURL=SignalrService.js.map
