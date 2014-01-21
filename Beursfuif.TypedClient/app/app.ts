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
    app.directive("bfBackground", [Background]);
    app.directive("bfModal", [Modal]);

    //routes
    app.config(($routeProvider: ng.route.IRouteProvider, $locationProvider: ng.ILocationProvider) => {


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
    });

};
//angular.module('MainModule', []).config(function ($locationProvider, $routeProvider) {
//    $locationProvider.hashPrefix("!");
//    $locationProvider.html5Mode(false);
//    $routeProvider.when('/', { template: './js/templates/index.html', controller: HelloWorldCtrl });
//    $routeProvider.when('/second', { template: './js/templates/second.html' });
//});