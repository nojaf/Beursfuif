/// <reference path="knockout-2.3.0.js" />
/// <reference path="../Viewmodels/Viewmodels.js" />
/// <reference path="../Models/models.js" />
/// <reference path="jquery-2.0.3.js" />
/// <reference path="moment.min.js" />



window.onload = initApp;

//GLOBALS
var app;


function initApp(e) {

    initBackground();

    initLoginScreen();

    app = {
        loginVM: new LoginViewModel(),
        drinksVM: new DrinkViewModel(),
        webSocket: null,
        orderVM: new OrderViewModel(),
        statusVM: new StatusViewModel(),
        errorModelVM:new ErrorModalViewModel()
    };

    ko.applyBindings(app.loginVM, document.getElementById("login"));
    ko.applyBindings(app.drinksVM, document.getElementById("drinks"));
    ko.applyBindings(app.orderVM, document.getElementById("order"));
    ko.applyBindings(app.statusVM, document.getElementById("status"));
    ko.applyBindings(app.errorModelVM, document.getElementById("errorModal"));

    checkForCachedValues();

    initErrorModal();
}

function initLoginScreen() {
    var login = document.getElementById("login").children[0];
    var screenHeight = document.documentElement.clientHeight;
    var offset = parseInt((screenHeight - login.clientHeight) / 2, 0);
    login.style.marginTop = offset + "px";
}

//#region Background
backGrounds = ["wallpaper-1113198.jpg",
                "wallpaper-123070.jpg",
                "wallpaper-1376284.jpg",
                "wallpaper-200796.jpg",
                "wallpaper-27328.jpg",
                "wallpaper-645423.jpg",
                "wallpaper-315078.jpg",
                "wallpaper-13780.jpg",
                "wallpaper-2962514.jpg",
                "wallpaper-473462.jpg",
                "wallpaper-1353552.jpg"];
currentWallpaperIndex = Math.round(Math.random() * (backGrounds.length - 1));

//no-repeat center center fixed;
function initBackground() {
    $("html").css("background", "url('wallpapers/" + backGrounds[currentWallpaperIndex] + "') no-repeat center center fixed");
    currentWallpaperIndex++;

    setInterval(changeBackground, 30*60*1000);
}

function changeBackground() {
    $("#mask").fadeIn(1000, function () {
        currentWallpaperIndex = (currentWallpaperIndex + 1) % backGrounds.length;
        
        $("html").css("background", "url('wallpapers/" + backGrounds[currentWallpaperIndex] + "') no-repeat center center fixed");
        $("#mask").fadeOut(1000);
    });
}


//#endregion

//#region Websocket
function initWebsocketMethodes(url) {
    app.webSocket = new WebSocket(url);
    app.webSocket.onopen = webSocketOpenHandler;
    app.webSocket.onclose = webSocketCloseHandler;
    app.webSocket.onmessage = webSocketMessageHandler;
    app.webSocket.onerror = webSocketErrorHandler;
}

function webSocketOpenHandler(e) {
    console.log("Connection established");
    $("#login").fadeOut(400);
    $("#mainControls").fadeIn(400);
    var initialPackage = new Package({ ClientName: app.loginVM.name() , MessageId:PROTOCOLKIND.NEW_CLIENT_CONNECTS});
    app.webSocket.send(JSON.stringify(initialPackage));
    cacheLoginValues(app.loginVM.name(), app.loginVM.serverAdress(), app.loginVM.port());
    //TODO: set Timer for ack
}

function webSocketCloseHandler(e) {
    console.log(e);
    app.errorModelVM.title("De verbinding met de server is weggevallen");
    app.errorModelVM.errorMessage("De connectie met de server is verloren. Controleer of het server programma nog actief is.");
    $("#errorModal").modal('show');
}

function webSocketMessageHandler(e) {
    var pack = new Package(JSON.parse(e.data));
    console.log("msgId = " + pack.MessageId);
    switch (pack.MessageId) {
        case PROTOCOLKIND.ACK_NEW_CLIENT_CONNECTS:
            ackNewClientConnects(pack);
            break;
        case PROTOCOLKIND.TIME_UPDATE:
            timeUpdate(pack);
            break;
        case PROTOCOLKIND.KICK_CLIENT:
            clientGotKicked();
            break;
    }
}

function webSocketErrorHandler(e) {
    console.log(e);
    app.errorModelVM.title("Onverwacht probleem opgedoken");
    app.errorModelVM.errorMessage("Er is een iets vreemd gebeurt, om veiligheidsredenen wordt de verbinding met de server verbroken. \nControleer of alles nog in orde is aan de serverkant en connecteer opnieuw.");
    $("#errorModal").modal('show');
}
//#endregion

//#region ReceivedWebSocketMessages
function ackNewClientConnects(pack) {
    app.statusVM.ClientId(pack.ClientId);

    console.log(pack.CurrentBeursfuifTime);
    var time = pack.CurrentBeursfuifTime.toString().split("T")[1].substr(0,5);

    app.statusVM.CurrentTime(time);

    var length = pack.CurrentInterval.ClientDrinks.length;
    for (var i = 0; i < length; i++) {
        app.drinksVM.drinks.push(pack.CurrentInterval.ClientDrinks[i]);
    }

    //image height:
    resizeImages(length);
}

function resizeImages(length) {
    var mainHeight = document.getElementById("mainControls").clientHeight;
    var spaceLeft = document.documentElement.clientHeight - document.getElementById("nav").clientHeight - mainHeight;
    console.log("spaceLeft  =" + spaceLeft);
    var rows = Math.ceil(length / 4);
    console.log("rows = " + rows);

    var currentHeight = $(".drink .row img").height();

    if (spaceLeft > 0) {


        var imageHeight = currentHeight +((spaceLeft - 20) / rows);
        console.log("imageHeight = " + imageHeight);
        $(".drink .row img").css("height", imageHeight + "px");

        /*var maxWidth = $(".drink .row").width();
        console.log("MaxWidth = " + maxWidth);
        var imgWidth = $(".drink .row img").width();
        console.log("image width = " + $(".drink .row img").css("width"));
        console.log((imgWidth - 30) + " > " + maxWidth);
        if ((imgWidth - 30) > maxWidth) {
            $(".drink .row img").css("height", "auto");
            $(".drink .row img").css("width", (maxWidth - 30) + "px");
            var minHeight = $($(".drink .row img")[0]).width();
            for (var j = 0; j < $(".drink .row img").length; j++) {
                var newHeight = $($(".drink .row img")[j]).width();
                if (minHeight > newHeight) {
                    minHeight = newHeight;
                }
            }
            $(".drink .row img").css("height", minHeight + "px");
            $(".drink .row img").css("width", "auto");
            $(".drink .row img").css("max-width", (maxWidth - 30) + "px");
        }*/
    } else if (spaceLeft < 0) {
        var smallerHeight = currentHeight + ((spaceLeft - 20) / 4);
        $(".drink .row img").css("height", smallerHeight + "px");
    }

}

function clientGotKicked() {
    //show error modal
    app.webSocket.close();
    app.errorModelVM.title("You got kicked from the server");
    app.errorModelVM.errorMessage("De server heeft je express ervan gesmeten.\nProbeer opnieuw verbinding te maken of vraag es beleefd waar de server hem eigenlijk moet hebben.");
    $("#errorModal").modal('show');

}

function timeUpdate(pack) {

    var time = pack.CurrentBeursfuifTime.toString().split("T")[1].substr(0, 5);
    console.log(time);
    app.statusVM.CurrentTime(time);

    var answerPack = new Package({
        MessageId: PROTOCOLKIND.ACK_TIME_UPDATE,
        ClientId: app.statusVM.ClientId(),
        AuthenticationCode: app.drinksVM.getAuthenticationCode()
    });
    
}
//#endregion

//#region cached values
function checkForCachedValues() {
    if (typeof (Storage) !== "undefined") {
        // Yes! localStorage and sessionStorage support!
        // Some code.....
        if (localStorage.getItem("name") !== undefined) {
            app.loginVM.name(localStorage.getItem("name"));
        }

        if (localStorage.getItem("serverAdress") !== undefined) {
            app.loginVM.serverAdress(localStorage.getItem("serverAdress"));
        }

        if (localStorage.getItem("port") !== undefined) {
            app.loginVM.port(localStorage.getItem("port"));
        }
    }
}

function cacheLoginValues(name, adress, port) {
    if (typeof (Storage) !== "undefined") {
        localStorage.name = name;
        localStorage.setItem("name", name);
        localStorage.setItem("serverAdress", adress);
        localStorage.setItem("port", port);
    }
}
//#endregion

function initErrorModal() {
    $('#errorModal').on('hidden.bs.modal', function () {
        resetAll();
    });
}

function resetAll() {
    app.drinksVM.drinks.removeAll();
    app.orderVM.items.removeAll();
    app.statusVM.ClientId(0);
    app.webSocket = null;
    $("#login").fadeIn(300);
    $("#mainControls").fadeOut(300);
}