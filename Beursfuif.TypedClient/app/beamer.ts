/// <reference path="_references.ts" />
module beursfuif {
    var app: ng.IModule = angular.module("beamer", ["ngRoute", "ngAnimate", "ngSanitize", "LocalStorageModule"]);
    //services
    app.factory("SignalrService", ["$q", "$log", "$rootScope", ($q: ng.IQService, $log: ng.ILogService, $rootScope: ng.IRootScopeService) => {
        return new SignalrService($q, $log, $rootScope);
    }]);

    app.controller("BeamerCtrl", ["$scope", "localStorageService", "SignalrService",
        ($scope: IBeamerCtrlScope, localStorageService: ILocalStorageService,
            signalrService: SignalrService) => {
            return new BeamerCtrl($scope, localStorageService, signalrService);
        }]);
}