/// <reference path="../app/_references.ts" />
module beursfuif {
    class SignalRMethodNames {
        //send to server
        static LOGIN: string = "logOn";

        //receiv from server
        static SEND_INITIAL_DATA: string = "sendInitialData";
    }

    export class SignalrService {

        constructor(private $q: ng.IQService, private $log: ng.ILogService, private $rootScope:ng.IRootScopeService) {}

        connection: HubConnection;

        hub: HubProxy;

        currentTime: Date;

        clientInterval: ClientInterval;

        initialize(url: string, name: string) {
            this.connection = $.hubConnection(url);
            this.hub = this.connection.createHubProxy("beursfuif");



            this.registerCallback();

            this.connection.start(() => {
                this.hub.invoke(SignalRMethodNames.LOGIN, name);
            });
        }



        registerCallback(): void {
            this.hub.on(SignalRMethodNames.SEND_INITIAL_DATA,(...msg:any[]) =>  this.sendInitialData(msg));
        }

        sendInitialData(...msg: any[]) {
            this.currentTime = <Date>msg[0][0];
            this.clientInterval = <ClientInterval>msg[0][1];
            this.$log.log(this.currentTime);
            this.$log.log(this.clientInterval);     

            this.$rootScope.$broadcast(EventNames.CONNECTION_CHANGED, true);    
        }
    }
}