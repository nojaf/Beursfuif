/// <reference path="../app/_references.ts" />

module beursfuif {
    export interface ILoginCtrlScope extends ng.IScope {
        ipAddress: string;
        port: string;
        name: string;
        submit: Function;
        connectionEstablished: any;
    }
    //localStorageService
    export class LoginCtrl {

        constructor(private $scope: ILoginCtrlScope, private localStorageService: ILocalStorageService, private signalrService:SignalrService, private $location:ng.ILocationService) {
            $scope.ipAddress = "192.168.1.11";
            $scope.port = "5678";
            $scope.name = "Florian";


            // $scope.$emit("CHANGE_OPACITY", 0.25);
            $scope.submit = () => {
                this.submit();
            };

            $scope.$on(EventNames.CONNECTION_CHANGED, (event: ng.IAngularEvent, ...args: any[]) => {
                if (args[0]) {
                    //TODO: store the credentials with localStorageService
                    $scope.$emit(EventNames.CHANGE_OPACITY, 0.50);
                    //connection has been made so the view can be changed
                    setTimeout(() => {
                        this.$location.path("/main");
                        this.$scope.$apply();
                    }, 500);
                }
            });
        }

        submit(): void {
            console.log("address : " + "http://" + this.$scope.ipAddress + ":" + this.$scope.port);
            this.signalrService.initialize("http://" + this.$scope.ipAddress + ":" + this.$scope.port, this.$scope.name);
        }

    }
}