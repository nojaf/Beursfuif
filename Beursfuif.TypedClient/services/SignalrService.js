/// <reference path="../app/_references.ts" />
var beursfuif;
(function (beursfuif) {
    var SignalRMethodNames = (function () {
        function SignalRMethodNames() {
        }
        SignalRMethodNames.LOGIN = "logOn";

        SignalRMethodNames.SEND_INITIAL_DATA = "sendInitialData";
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
        };

        SignalrService.prototype.sendInitialData = function () {
            var msg = [];
            for (var _i = 0; _i < (arguments.length - 0); _i++) {
                msg[_i] = arguments[_i + 0];
            }
            this.currentTime = msg[0][0];
            this.clientInterval = msg[0][1];
            this.$log.log(this.currentTime);
            this.$log.log(this.clientInterval);

            this.$rootScope.$broadcast(beursfuif.EventNames.CONNECTION_CHANGED, true);
        };
        return SignalrService;
    })();
    beursfuif.SignalrService = SignalrService;
})(beursfuif || (beursfuif = {}));
//# sourceMappingURL=SignalrService.js.map
