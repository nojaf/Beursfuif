/// <reference path="../app/_references.ts" />
var bokken;
(function (bokken) {
    var connection = $.hubConnection("http://192.168.1.11:5678");
    console.log(connection);
    var hub = connection.createHubProxy("beursfuif");
    console.log(hub);
    connection.start().then(function () {
        hub.invoke("logOn", "test.html");
    });
})(bokken || (bokken = {}));
//# sourceMappingURL=test.js.map
