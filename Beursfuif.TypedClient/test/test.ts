/// <reference path="../app/_references.ts" />
module bokken {
    var connection: HubConnection = $.hubConnection("http://192.168.1.11:5678");
    console.log(connection);
    var hub: HubProxy = connection.createHubProxy("beursfuif");
    console.log(hub);
    connection.start().then(function () {
        hub.invoke("logOn", "test.html");
    });
} 