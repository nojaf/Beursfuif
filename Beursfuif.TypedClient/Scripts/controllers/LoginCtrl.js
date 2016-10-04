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
            $scope.name = this.localStorageService.get("name") || "";
            $scope.vm = this;
            $scope.$on(beursfuif.EventNames.CONNECTION_CHANGED, function (event) {
                var args = [];
                for (var _i = 1; _i < arguments.length; _i++) {
                    args[_i - 1] = arguments[_i];
                }
                if (args[0]) {
                    if (_this.localStorageService.isSupported) {
                        _this.localStorageService.add("name", _this.$scope.name);
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
            var address = "http://" + location.host;
            console.log("address : " + address);
            this.signalrService.initialize(address, this.$scope.name);
            this.$scope.isLoading = true;
        };
        return LoginCtrl;
    }());
    beursfuif.LoginCtrl = LoginCtrl;
    beursfuif.beursfuifModule.controller("LoginCtrl", ["$scope", "localStorageService", "SignalrService", "$location", "$timeout",
        function ($scope, localStorageService, signalrService, $location, $timeout) {
            return new LoginCtrl($scope, localStorageService, signalrService, $location, $timeout);
        }]);
})(beursfuif || (beursfuif = {}));
//# sourceMappingURL=LoginCtrl.js.map