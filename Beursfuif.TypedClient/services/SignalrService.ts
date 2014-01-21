/// <reference path="../app/_references.ts" />
module beursfuif {
    class SignalRMethodNames {
        //send to server
        static LOGIN: string = "logOn";

        //receiv from server
        static SEND_INITIAL_DATA: string = "sendInitialData";
        static UPDATE_TIME:string  = "updateTime"//(currentTime, authenticationCode);
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
            this.hub.on(SignalRMethodNames.SEND_INITIAL_DATA, (...msg: any[]) => this.sendInitialData(msg));
            this.hub.on(SignalRMethodNames.UPDATE_TIME, (...msg: any[]) => this.updateTime(msg));
        }

        sendInitialData(...msg: any[]) {
            this.currentTime = <Date>msg[0][0];
            this.clientInterval = <ClientInterval>msg[0][1];
            this.clientInterval.ClientDrinks.sort(this.sortByLowerDrinkName);
            this.$log.log(this.currentTime);
            this.$log.log(this.clientInterval);     

            this.$rootScope.$broadcast(EventNames.CONNECTION_CHANGED, true);    
        }

        updateTime(...msg: any[]) {
            this.$log.log("time time time");
            this.$log.log(msg);

            this.validateAuthString(msg[0][1]);

            this.currentTime = msg[0][0];
            this.$rootScope.$broadcast(EventNames.TIME_CHANGED);
        }

        //#region Authcode
        validateAuthString(serverAuthString: string) {
            var clientAuthString: string = this.generateAuthString();
            this.$log.log("Client auth = " + clientAuthString);
            this.$log.log("Received server auth = " + serverAuthString);
            if (clientAuthString === serverAuthString) {
                this.$log.log("Auth code mathces");
            }
            else {
                this.$log.error("Wrong auth code");
                //TODO: show modal that says you're kicked, ...
                this.connection.stop(false, true);
                this.$rootScope.$broadcast(EventNames.OPEN_MODAL, "Verkeerde authenticatie code", "Je authenticatie code komt niet overeen met die van de server. " +
                    "<br />Dit wil zeggen dat je niet de juiste prijzen hebt."+
                    "<br />De connectie met de server wordt verbroken."+
                    "<br />gelieve opnieuw aan te melden.");
            }
        }

        generateAuthString(): string {
            var length: number = this.clientInterval.ClientDrinks.length;
            var clientAuth: string = "(" + this.clientInterval.Id + ")";
            for (var i: number = 0; i < length; i++) {
                clientAuth = clientAuth + "[" + this.clientInterval.ClientDrinks[i].DrinkId + ":" + this.clientInterval.ClientDrinks[i].Price + "]";
            }
            return clientAuth;
        }

        sortByLowerDrinkName(a: ClientDrink, b: ClientDrink):number {
            if (a.Name.toLowerCase() > b.Name.toLowerCase()) {
                return 1;
            } else if (a.Name.toLowerCase() < b.Name.toLowerCase()) {
                return -1;
            }
            return 0;
        }
        //#endregion
    }
}