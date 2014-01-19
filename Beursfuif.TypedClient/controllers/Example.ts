// Update the reference to app1.ts to be that of your module file.
// Install the angularjs.TypeScript.DefinitelyTyped NuGet package to resovle the .d.ts reference paths,
// then adjust the path value to be relative to this file
/*

interface IExampleScope extends ng.IScope {
    greeting: string;
    changeGreeting: () => void;
}

interface IExample {
    controllerId: string;
}

class Example implements IExample {
    static controllerId: string = "Example";
    
    constructor(private $scope: IExampleScope, private $http: ng.IHttpService, private $resource: ng.resource.IResourceService) {
        $scope.greeting = "Hello";
        $scope.changeGreeting = () => this.changeGreeting();
    }

    private changeGreeting() {
        this.$scope.greeting = "Bye";
    }
}

// Update the app1 variable name to be that of your module variable
app1.controller(Example.controllerId, ['$scope', '$http', '$resource', function ($scope, $http, $resource) {
    return new Example($scope, $http, $resource);
}]);
*/