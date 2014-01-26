/// <reference path="../app/_references.ts" />
module beursfuif {
    class SignalRMethodNames {
        //#region send to server
        static LOGIN: string = "logOn";
        static ACK_TIME_UPDATE: string = "ackTimeUpdate";
        static NEW_ORDER: string = "newOrder";
        static ACK_INTERVAL_UPDATE: string = "ackIntervalUpdate";
        //#endregion

        //#region receive from server
        static SEND_INITIAL_DATA: string = "sendInitialData";
        static UPDATE_TIME: string = "updateTime"//(currentTime, authenticationCode);
        static YOU_GOT_KICKED: string = "youGotKicked";
        static ACK_NEW_ORDER: string = "ackNewOrder";
        static UPDATE_INTERVAL: string = "updateInterval"; //(clientInterval, currentBFTime);
        //#endregion
    }

    export class SignalrService {
        //#region fields
        connection: HubConnection;

        hub: HubProxy;

        currentTime: Date;

        clientInterval: IClientInterval;
        //#endregion

        constructor(private $q: ng.IQService, private $log: ng.ILogService, private $rootScope: ng.IRootScopeService) { }

        initialize(url: string, name: string) {
            this.connection = $.hubConnection(url);
            this.hub = this.connection.createHubProxy("beursfuif");

            this.registerCallback();

            this.connection.error(() => {
                this.connection.stop(false, false);
                this.$rootScope.$broadcast(EventNames.OPEN_MODAL, ModalMessages.CONNECTION_LOST_TITLE, ModalMessages.CONNECTION_LOST);
            });

            this.connection.start(() => {
                console.log("start");
                this.hub.invoke(SignalRMethodNames.LOGIN, name);
            }).fail(() => {
                console.log("fail");
                this.$rootScope.$broadcast(EventNames.OPEN_MODAL, ModalMessages.CONNECTION_LOST_TITLE, ModalMessages.CONNECTION_LOST);
            });
        }

        //#region callbacks from the server
        registerCallback(): void {
            this.hub.on(SignalRMethodNames.SEND_INITIAL_DATA, (...msg: any[]) => this.sendInitialData(msg));
            this.hub.on(SignalRMethodNames.UPDATE_TIME, (...msg: any[]) => this.updateTime(msg));
            this.hub.on(SignalRMethodNames.YOU_GOT_KICKED, () => this.kicked());
            this.hub.on(SignalRMethodNames.ACK_NEW_ORDER, () => this.showToast());
            this.hub.on(SignalRMethodNames.UPDATE_INTERVAL, (...msg: any[]) => this.updateInterval(msg));
        }

        sendInitialData(...msg: any[]) {
            this.currentTime = <Date>msg[0][0];
            this.clientInterval = <IClientInterval>msg[0][1];
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

        kicked(): void {
            this.connection.stop(false, true);
            this.$rootScope.$broadcast(EventNames.OPEN_MODAL, ModalMessages.YOU_GOT_KICKED_TITLE, ModalMessages.YOU_GOT_KICKED);
        }

        showToast(): void {
            toastr.success("Je bestelling werd goed ontvangen.","Bestelling gelukt!");
        }

        updateInterval(...msg: any[]) {
            this.$log.log(msg);
            this.currentTime = <Date>msg[0][1];
            this.clientInterval = <IClientInterval>msg[0][0];
            this.clientInterval.ClientDrinks.sort(this.sortByLowerDrinkName);
            this.$rootScope.$broadcast(EventNames.INTERVAL_UPDATE);
            toastr.info("De prijzen werden aangepast", "Prijzen update.");

            //respond to server
            this.hub.invoke(SignalRMethodNames.ACK_INTERVAL_UPDATE, this.generateAuthString());
        }
        //#endregion

        //#region Authcode
        validateAuthString(serverAuthString: string) {
            var clientAuthString: string = this.generateAuthString();
            if (clientAuthString === serverAuthString) {
                this.$log.log("Auth code mathces");
                //respond to the incoming message
                this.hub.invoke(SignalRMethodNames.ACK_TIME_UPDATE, clientAuthString);
            }
            else {
                this.$log.warn("Wrong auth code");
                this.connection.stop(false, true);
                this.$rootScope.$broadcast(EventNames.OPEN_MODAL, ModalMessages.WRONG_AUTH_TITLE, ModalMessages.WRONG_AUTH);
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

        sortByLowerDrinkName(a: IClientDrink, b: IClientDrink):number {
            if (a.Name.toLowerCase() > b.Name.toLowerCase()) {
                return 1;
            } else if (a.Name.toLowerCase() < b.Name.toLowerCase()) {
                return -1;
            }
            return 0;
        }
        //#endregion

        //#region Send to the server
        sendNewOrder(order: Array<IClientDrinkOrder>) {
            this.hub.invoke(SignalRMethodNames.NEW_ORDER, order, this.generateAuthString());
        }
        //#endregion
    }
}