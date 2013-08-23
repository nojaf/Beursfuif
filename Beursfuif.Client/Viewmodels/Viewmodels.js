/// <reference path="../Models/models.js" />
/// <reference path="../Scripts/knockout-2.3.0.js" />
/// <reference path="../Scripts/jquery-2.0.3.js" />
/// <reference path="../Scripts/app.js" />

//#region LOGIN
function LoginViewModel() {

    this.name = ko.observable("Florian");
    this.serverAdress = ko.observable("192.168.1.100");
    this.port = ko.observable("5678");

    this.isValid = ko.computed(function () {
        //Name
        if (this.name().trim().length === 0) return false;

        //Adress
        if (this.serverAdress().trim().length === 0) return false;
      
        console.log("validate  = " + validate_ip(this.serverAdress()));
        if (!validate_ip(this.serverAdress())) return false;

        //Port
        if (this.port().trim().length === 0) return false;
        if (!parseInt(this.port(), 0)) return false;

        return true;
    }, this);

    this.connectToServer = function (e) {
        try {
            initWebsocketMethodes("ws://" + this.serverAdress() + ":" + this.port());
        }
        catch (ex) {
            alert("Kon niet verbinden met de server: " + ex.message);
        }
    };
}


function validate_ip(ip) {
    // See if x looks like an IP address using our "almost IP regex".
    var regex = /^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$/;
    var match = regex.exec(ip);
    if (!match) return false;
    // Additional code to check that the octets aren't greater than 255:
    for (var i = 1; i <= 4; ++i) {
        if (parseInt(match[i],0) > 255)
            return false;
    }
    return true;
}
//#endregion

//#region DRINKS
function DrinkViewModel() {
    this.drinks = ko.observableArray();

}
//#endregion