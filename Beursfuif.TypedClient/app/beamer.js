/// <reference path="_references.ts" />
var beursfuif;
(function (beursfuif) {
    var app = angular.module("beamer", ["ngRoute", "ngAnimate", "ngSanitize", "LocalStorageModule"]);

    //services
    app.factory("SignalrService", [
        "$q", "$log", "$rootScope", function ($q, $log, $rootScope) {
            return new beursfuif.SignalrService($q, $log, $rootScope);
        }]);

    app.controller("BeamerCtrl", [
        "$scope", "localStorageService", "SignalrService",
        function ($scope, localStorageService, signalrService) {
            return new beursfuif.BeamerCtrl($scope, localStorageService, signalrService);
        }]);
})(beursfuif || (beursfuif = {}));
//# sourceMappingURL=beamer.js.map
