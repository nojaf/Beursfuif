/// <reference path="../app/_references.ts" />
module beursfuif {
    class MethodNames {
        static LOGIN: string = "logOn";
    }

    export class SignalrService {

        connection: HubConnection;

        hub: HubProxy;

        initialize(url:string, name:string) {
            this.connection = $.hubConnection(url);
            this.hub = this.connection.createHubProxy("beursfuif");
            this.connection.start(() => {
                this.hub.invoke(MethodNames.LOGIN, name);
            });
        }


    }
}