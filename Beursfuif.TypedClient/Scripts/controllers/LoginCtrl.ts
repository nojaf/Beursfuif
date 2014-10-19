module beursfuif {
    export interface ILoginCtrlScope extends ng.IScope {
        ipAddress: string;
        port: string;
        name: string;
        vm: LoginCtrl;
        connectionEstablished: any;
        isLoading: boolean;

    }
    //localStorageService
    export class LoginCtrl {

        constructor(private $scope: ILoginCtrlScope, private localStorageService: ILocalStorageService,
            private signalrService: SignalrService, private $location: ng.ILocationService, private $timeout:ng.ITimeoutService) {
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
                    $timeout(() => {
                        this.$scope.isLoading = false;
                        this.$location.path("/main");
                    }, 250);
                }
            });

            this.$scope.$on(EventNames.OPEN_MODAL, (e: ng.IAngularEvent) => {
                //something went wrong (most likely wrong ip)
                this.$scope.isLoading = false;
            });
        }

        submit(): void {
            console.log("address : " + "http://" + this.$scope.ipAddress + ":" + this.$scope.port);
            this.signalrService.initialize("http://" + this.$scope.ipAddress + ":" + this.$scope.port, this.$scope.name);
            this.$scope.isLoading = true;
        }

    }

    beursfuifModule.controller("LoginCtrl", ["$scope", "localStorageService", "SignalrService", "$location","$timeout",
        ($scope: ILoginCtrlScope, localStorageService: ILocalStorageService,
            signalrService: SignalrService, $location: ng.ILocationService,
            $timeout:ng.ITimeoutService) => {
            return new LoginCtrl($scope, localStorageService, signalrService, $location, $timeout);
        }]);
}

