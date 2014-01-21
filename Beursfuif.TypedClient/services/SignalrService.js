/// <reference path="../app/_references.ts" />
var beursfuif;
(function (beursfuif) {
    var SignalRMethodNames = (function () {
        function SignalRMethodNames() {
        }
        SignalRMethodNames.LOGIN = "logOn";

        SignalRMethodNames.SEND_INITIAL_DATA = "sendInitialData";
        SignalRMethodNames.UPDATE_TIME = "updateTime";
        return SignalRMethodNames;
    })();

    var SignalrService = (function () {
        function SignalrService($q, $log, $rootScope) {
            this.$q = $q;
            this.$log = $log;
            this.$rootScope = $rootScope;
        }
        SignalrService.prototype.initialize = function (url, name) {
            var _this = this;
            this.connection = $.hubConnection(url);
            this.hub = this.connection.createHubProxy("beursfuif");

            this.registerCallback();

            this.connection.start(function () {
                _this.hub.invoke(SignalRMethodNames.LOGIN, name);
            });
        };

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

        //#region Authcode
        SignalrService.prototype.validateAuthString = function (serverAuthString) {
            var clientAuthString = this.generateAuthString();
            this.$log.log("Client auth = " + clientAuthString);
            this.$log.log("Received server auth = " + serverAuthString);
            if (clientAuthString === serverAuthString) {
                this.$log.log("Auth code mathces");
            } else {
                this.$log.error("Wrong auth code");

                //TODO: show modal that says you're kicked, ...
                this.connection.stop(false, true);
                this.$rootScope.$broadcast(beursfuif.EventNames.OPEN_MODAL, "Verkeerde authenticatie code", "Je authenticatie code komt niet overeen met die van de server. " + "<br />Dit wil zeggen dat je niet de juiste prijzen hebt." + "<br />De connectie met de server wordt verbroken." + "<br />gelieve opnieuw aan te melden.");
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
        return SignalrService;
    })();
    beursfuif.SignalrService = SignalrService;
})(beursfuif || (beursfuif = {}));
//# sourceMappingURL=SignalrService.js.map
