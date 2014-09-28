module beursfuif {
    export interface ILoginCtrlScope extends ng.IScope {
        ipAddress: string;
        port: string;
        name: string;
        vm: LoginCtrl;
        connectionEstablished: any;

    }
    //localStorageService
    export class LoginCtrl {

        constructor(private $scope: ILoginCtrlScope, private localStorageService: ILocalStorageService, private signalrService:SignalrService, private $location:ng.ILocationService) {
            $scope.ipAddress = this.localStorageService.get("ipAddress") || "";
            $scope.port = this.localStorageService.get("port") || "";
            $scope.name = this.localStorageService.get("name") || "";

            $scope.vm = this;

            $scope.$on(EventNames.CONNECTION_CHANGED, (event: ng.IAngularEvent, ...args: any[]) => {
                if (args[0]) {
                    if (this.localStorageService.isSupported) {
                        this.localStorageService.add("name", this.$scope.name);
                        this.localStorageService.add("ipAddress", this.$scope.ipAddress);
                        this.localStorageService.add("port", this.$scope.port);
                    }

                    $scope.$emit(EventNames.CHANGE_OPACITY, 0.50);
                    //connection has been made so the view can be changed
                    setTimeout(() => {
                        this.$location.path("/main");
                        this.$scope.$apply();
                    }, 250);
                }
            });
        }

        submit(): void {
            console.log("address : " + "http://" + this.$scope.ipAddress + ":" + this.$scope.port);
            this.signalrService.initialize("http://" + this.$scope.ipAddress + ":" + this.$scope.port, this.$scope.name);
        }

    }

    beursfuifModule.controller("LoginCtrl", ["$scope", "localStorageService", "SignalrService", "$location",
        ($scope: ILoginCtrlScope, localStorageService: ILocalStorageService,
            signalrService: SignalrService, $location: ng.ILocationService) => {
            return new LoginCtrl($scope, localStorageService, signalrService, $location);
        }]);
}

