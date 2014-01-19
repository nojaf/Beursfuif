/// <reference path="../app/_references.ts" />
var beursfuif;
(function (beursfuif) {
    //localStorageService
    var LoginCtrl = (function () {
        function LoginCtrl($scope, localStorageService, signalrService, $location) {
            var _this = this;
            this.$scope = $scope;
            this.localStorageService = localStorageService;
            this.signalrService = signalrService;
            this.$location = $location;
            $scope.ipAddress = "192.168.1.11";
            $scope.port = "5678";
            $scope.name = "Florian";

            // $scope.$emit("CHANGE_OPACITY", 0.25);
            $scope.submit = function () {
                _this.submit();
            };

            $scope.$on(beursfuif.EventNames.CONNECTION_CHANGED, function (event) {
                var args = [];
                for (var _i = 0; _i < (arguments.length - 1); _i++) {
                    args[_i] = arguments[_i + 1];
                }
                if (args[0]) {
                    //TODO: store the credentials with localStorageService
                    $scope.$emit(beursfuif.EventNames.CHANGE_OPACITY, 0.50);

                    //connection has been made so the view can be changed
                    setTimeout(function () {
                        _this.$location.path("/main");
                        _this.$scope.$apply();
                    }, 500);
                }
            });
        }
        LoginCtrl.prototype.submit = function () {
            console.log("address : " + "http://" + this.$scope.ipAddress + ":" + this.$scope.port);
            this.signalrService.initialize("http://" + this.$scope.ipAddress + ":" + this.$scope.port, this.$scope.name);
        };
        return LoginCtrl;
    })();
    beursfuif.LoginCtrl = LoginCtrl;
})(beursfuif || (beursfuif = {}));
//# sourceMappingURL=LoginCtrl.js.map
