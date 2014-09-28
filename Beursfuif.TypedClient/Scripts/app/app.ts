module beursfuif {
    export var beursfuifModule: ng.IModule = angular.module("beursfuif", ["ngRoute", "ngAnimate", "ngSanitize", "LocalStorageModule"]);

    //routes
    beursfuifModule.config(['$routeProvider', ($routeProvider: ng.route.IRouteProvider) => {

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
    }]);

};
