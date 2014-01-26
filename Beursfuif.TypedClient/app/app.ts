/// <reference path="_references.ts" />
module beursfuif {
    var app: ng.IModule = angular.module("beursfuif", ["ngRoute", "ngAnimate", "ngSanitize", "LocalStorageModule"]);
    //services
    app.factory("SignalrService", ["$q", "$log","$rootScope", ($q: ng.IQService, $log: ng.ILogService, $rootScope:ng.IRootScopeService) => {
        return new SignalrService($q, $log, $rootScope);
    }]);

 

    //controllers
    app.controller("LoginCtrl", ["$scope", "localStorageService", "SignalrService","$location",
        ($scope: ILoginCtrlScope, localStorageService: ILocalStorageService,
            signalrService: SignalrService, $location:ng.ILocationService) => {
        return new LoginCtrl($scope, localStorageService, signalrService, $location);
        }]);

    app.controller("MainCtrl", ["$scope", "SignalrService", "$location",
        ($scope: IMainCtrlScope, signalrService: SignalrService, $location: ng.ILocationService) => {
            return new MainCtrl($scope, signalrService, $location);
        }]);


    //directives

    // Update the app1 variable name to be that of your module variable
    app.directive("bfBackground", [() => { return new Background(); }]);
    app.directive("bfModal", [() => { return new beursfuif.Modal(); }]);

    //routes
    app.config(($routeProvider: ng.route.IRouteProvider, $locationProvider: ng.ILocationProvider) => {

        //routes
        $routeProvider
            .when("/", {
                templateUrl: "loginView",
                controller: "LoginCtrl"
            })
            .when("/main", {
                templateUrl: "mainView",
                controller:"MainCtrl"
            })
            .otherwise({
                redirectTo: "/"
            });

        //toastr
        toastr.options = {
                closeButton: false,
                debug: false,
                positionClass: "toast-top-right",
                onclick: null,
                showDuration: 300,
                hideDuration: 300,
                timeOut: 1000,
                extendedTimeOut: 1000,
                showEasing: "swing",
                hideEasing: "linear",
                showMethod: "fadeIn",
                hideMethod: "fadeOut"
            };
    });

};
