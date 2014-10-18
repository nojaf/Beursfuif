module beursfuif {
    export class SignalRMethodNames {
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
        static DRINK_AVAILABLE_CHANGED: string = "drinkAvailableChanged";
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
            $.connection.hub.logging = true;

            this.registerCallback();

            this.connection.error(() => {
                this.connection.stop(false, false);
                this.unregisterCallbacks();
                this.$rootScope.$broadcast(EventNames.OPEN_MODAL, ModalMessages.CONNECTION_LOST_TITLE, ModalMessages.CONNECTION_LOST);
            });

            this.connection.start(() => {
                this.$log.info("start");
                this.hub.invoke(SignalRMethodNames.LOGIN, name);
            }).fail((e) => {
                    console.log("fail", e);
                    this.$rootScope.$broadcast(EventNames.OPEN_MODAL, ModalMessages.CONNECTION_LOST_TITLE, ModalMessages.CONNECTION_LOST);
                });
            
        }

        //#region callbacks from the server
        unregisterCallbacks() {
            this.hub.off(SignalRMethodNames.SEND_INITIAL_DATA, (msg) => { this.$log.info("Off initial data")});
            this.hub.off(SignalRMethodNames.UPDATE_TIME, (msg) => { this.$log.info("Off update time") });
            this.hub.off(SignalRMethodNames.YOU_GOT_KICKED, (msg) => { this.$log.info("Off you got kicked") });
            this.hub.off(SignalRMethodNames.ACK_NEW_ORDER, (msg) => { this.$log.info("Off ack new order") });
            this.hub.off(SignalRMethodNames.UPDATE_INTERVAL, (msg) => { this.$log.info("Off update interval") });
            this.hub.off(SignalRMethodNames.DRINK_AVAILABLE_CHANGED, (msg) => { this.$log.info("Off drink available") });
        }

        registerCallback(): void {
            this.hub.on(SignalRMethodNames.SEND_INITIAL_DATA, (currentTime: Date, clientInterval: IClientInterval)
                => this.sendInitialData(currentTime, clientInterval));
            this.hub.on(SignalRMethodNames.UPDATE_TIME, (currentTime: Date, authenticationCode: string) => this.updateTime(currentTime, authenticationCode));
            this.hub.on(SignalRMethodNames.YOU_GOT_KICKED, () => this.kicked());
            this.hub.on(SignalRMethodNames.ACK_NEW_ORDER, () => this.showToast());
            this.hub.on(SignalRMethodNames.UPDATE_INTERVAL, (clientInterval: IClientInterval, currentBFTime: Date) => this.updateInterval(clientInterval, currentBFTime));
            this.hub.on(SignalRMethodNames.DRINK_AVAILABLE_CHANGED, (clientInterval: IClientInterval) => this.drinkAvailableChanged(clientInterval));
        }

        sendInitialData(currentTime:Date, clientInterval:IClientInterval) {
            console.log("initial data", currentTime, clientInterval);
            this.currentTime = currentTime;
            this.clientInterval = clientInterval;
            this.clientInterval.ClientDrinks.sort(this.sortByLowerDrinkName);
            this.$log.log(this.currentTime);
            this.$log.log(this.clientInterval);     
            this.$rootScope.$broadcast(EventNames.CONNECTION_CHANGED, true);    
        }

        updateTime(currentTime:Date, authenticationCode:string) {
            this.$log.log("time time time", currentTime);

            this.validateAuthString(authenticationCode);

            this.currentTime = currentTime;
            this.$rootScope.$broadcast(EventNames.TIME_CHANGED);
        }

        kicked(): void {

            this.hub.connection.stop(false, true);
            this.$rootScope.$broadcast(EventNames.OPEN_MODAL, ModalMessages.YOU_GOT_KICKED_TITLE, ModalMessages.YOU_GOT_KICKED);

            this.hub = null;
            this.connection = null;
        }

        showToast(): void {
            toastr.success("Je bestelling werd goed ontvangen.","Bestelling gelukt!");
        }

        updateInterval(clientInterval:IClientInterval, currentBFTime:Date) {
            this.$log.log(clientInterval, currentBFTime);
            this.currentTime = currentBFTime;
            this.clientInterval = clientInterval;
            this.clientInterval.ClientDrinks.sort(this.sortByLowerDrinkName);
            this.$rootScope.$broadcast(EventNames.INTERVAL_UPDATE);
            toastr.info("De prijzen werden aangepast", "Prijzen update.");

            //respond to server
            this.hub.invoke(SignalRMethodNames.ACK_INTERVAL_UPDATE, this.generateAuthString());
        }

        drinkAvailableChanged(clientInterval: IClientInterval) {
            console.log(clientInterval);
            this.currentTime = clientInterval.CurrentTime;
            this.clientInterval = clientInterval;
            this.clientInterval.ClientDrinks.sort(this.sortByLowerDrinkName);
            this.$rootScope.$broadcast(EventNames.DRINK_AVAILABLE_CHANGED);
            toastr.info("Het aantal beschikbare dranken is veranderd", "Drank beschikbaarheid aangepast.");
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

    if (beamerModule) {
        beamerModule.factory("SignalrService", ["$q", "$log", "$rootScope", ($q: ng.IQService, $log: ng.ILogService, $rootScope: ng.IRootScopeService) => {
            return new SignalrService($q, $log, $rootScope);
        }]);
    }

    if (beursfuifModule) {
        beursfuifModule.factory("SignalrService", ["$q", "$log", "$rootScope", ($q: ng.IQService, $log: ng.ILogService, $rootScope: ng.IRootScopeService) => {
            return new SignalrService($q, $log, $rootScope);
        }]);
    }
}