/// <reference path="_references.ts" />
var beursfuif;
(function (beursfuif) {
    var app = angular.module("beursfuif", ["ngRoute", "ngAnimate", "ngSanitize", "LocalStorageModule"]);

    //services
    app.factory("SignalrService", [
        "$q", "$log", "$rootScope", function ($q, $log, $rootScope) {
            return new beursfuif.SignalrService($q, $log, $rootScope);
        }]);

    //controllers
    app.controller("LoginCtrl", [
        "$scope", "localStorageService", "SignalrService", "$location",
        function ($scope, localStorageService, signalrService, $location) {
            return new beursfuif.LoginCtrl($scope, localStorageService, signalrService, $location);
        }]);

    app.controller("MainCtrl", [
        "$scope", "SignalrService", "$location",
        function ($scope, signalrService, $location) {
            return new beursfuif.MainCtrl($scope, signalrService, $location);
        }]);

    //directives
    // Update the app1 variable name to be that of your module variable
    app.directive("bfBackground", [function () {
            return new beursfuif.Background();
        }]);
    app.directive("bfModal", [function () {
            return new beursfuif.Modal();
        }]);

    //routes
    app.config(function ($routeProvider, $locationProvider) {
        //routes
        $routeProvider.when("/", {
            templateUrl: "loginView",
            controller: "LoginCtrl"
        }).when("/main", {
            templateUrl: "mainView",
            controller: "MainCtrl"
        }).otherwise({
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
})(beursfuif || (beursfuif = {}));
;
//# sourceMappingURL=app.js.map
