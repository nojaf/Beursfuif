module beursfuif {
    export interface IClientInterval {
        /*		ClientDrinks: Array[1]
        CurrentTime: "1970-01-01T21:00:21"
        End: "1970-01-01T21:15:00"
        Id: 1
        Start: "1970-01-01T21:00:00"*/
        ClientDrinks: IClientDrink[];
        CurrentTime: Date;
        End: Date;
        Id: number;
        Start: Date;
    }

    export interface IClientDrink  {
        /*   Base64Image: "iVBORw0KGgoAAACC"
               DrinkId: 1
               IntervalId: 1
               Name: "Whatever"
               Price: 14*/
        Base64Image: string;
        DrinkId: number;
        IntervalId: number;
        Name: string;
        Price: number;
    }

    export interface IClientDrinkOrder {
        DrinkId: number;
        Count: number;
        IntervalId: number;
        Name:string;
    }
} 