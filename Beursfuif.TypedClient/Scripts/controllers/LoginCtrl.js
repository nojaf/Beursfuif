var beursfuif;
(function (beursfuif) {
    //localStorageService
    var LoginCtrl = (function () {
        function LoginCtrl($scope, localStorageService, signalrService, $location, $timeout) {
            var _this = this;
            this.$scope = $scope;
            this.localStorageService = localStorageService;
            this.signalrService = signalrService;
            this.$location = $location;
            this.$timeout = $timeout;
            $scope.ipAddress = this.localStorageService.get("ipAddress") || "";
            $scope.port = this.localStorageService.get("port") || "";
            $scope.name = this.localStorageService.get("name") || "";

            $scope.vm = this;

            $scope.$on(beursfuif.EventNames.CONNECTION_CHANGED, function (event) {
                var args = [];
                for (var _i = 0; _i < (arguments.length - 1); _i++) {
                    args[_i] = arguments[_i + 1];
                }
                if (args[0]) {
                    if (_this.localStorageService.isSupported) {
                        _this.localStorageService.add("name", _this.$scope.name);
                        _this.localStorageService.add("ipAddress", _this.$scope.ipAddress);
                        _this.localStorageService.add("port", _this.$scope.port);
                    }

                    $scope.$emit(beursfuif.EventNames.CHANGE_OPACITY, 0.50);

                    //connection has been made so the view can be changed
                    $timeout(function () {
                        _this.$scope.isLoading = false;
                        _this.$location.path("/main");
                    }, 250);
                }
            });

            this.$scope.$on(beursfuif.EventNames.OPEN_MODAL, function (e) {
                //something went wrong (most likely wrong ip)
                _this.$scope.isLoading = false;
            });
        }
        LoginCtrl.prototype.submit = function () {
            console.log("address : " + "http://" + this.$scope.ipAddress + ":" + this.$scope.port);
            this.signalrService.initialize("http://" + this.$scope.ipAddress + ":" + this.$scope.port, this.$scope.name);
            this.$scope.isLoading = true;
        };
        return LoginCtrl;
    })();
    beursfuif.LoginCtrl = LoginCtrl;

    beursfuif.beursfuifModule.controller("LoginCtrl", [
        "$scope", "localStorageService", "SignalrService", "$location", "$timeout",
        function ($scope, localStorageService, signalrService, $location, $timeout) {
            return new LoginCtrl($scope, localStorageService, signalrService, $location, $timeout);
        }]);
})(beursfuif || (beursfuif = {}));
//# sourceMappingURL=LoginCtrl.js.map
