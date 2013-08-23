/// <reference path="knockout-2.3.0.js" />
/// <reference path="../Viewmodels/Viewmodels.js" />
/// <reference path="../Models/models.js" />
/// <reference path="jquery-2.0.3.js" />


window.onload = initApp;

//GLOBALS
var loginVM;
var drinksVM;
var webSocket;

function initApp(e) {

    initBackground();

    initLoginScreen();

    loginVM = new LoginViewModel();
    drinksVM = new DrinkViewModel();

    ko.applyBindings(loginVM, document.getElementById("login"));
    ko.applyBindings(drinksVM, document.getElementById("drinks"));

    checkForCachedValues();
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
                "wallpaper-462260.jpg",
                "wallpaper-1007744.jpg",
                "wallpaper-13780.jpg",
                "wallpaper-2962514.jpg",
                "wallpaper-473462.jpg"];
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
    webSocket = new WebSocket(url);
    webSocket.onopen = webSocketOpenHandler;
    webSocket.onclose = webSocketCloseHandler;
    webSocket.onmessage = webSocketMessageHandler;
    webSocket.onerror = webSocketErrorHandler;
}

function webSocketOpenHandler(e) {
    console.log("Connection established");
    $("#login").fadeOut(400);
    $("#mainControls").fadeIn(400);
    var initialPackage = new Package({ ClientName: loginVM.name() , MessageId:1});
    webSocket.send(JSON.stringify(initialPackage));
    cacheLoginValues(loginVM.name(), loginVM.serverAdress(), loginVM.port());
    //TODO: set Timer for ack
}

function webSocketCloseHandler(e) {
    console.log(e);
}

function webSocketMessageHandler(e) {
    var pack = new Package(JSON.parse(e.data));
    console.log("msgId = " + pack.MessageId);
    switch (pack.MessageId) {
        case PROTOCOLKIND.ACK_NEW_CLIENT_CONNECTS:
            ackNewClientConnects(pack);
            break;

    }
}

function webSocketErrorHandler(e) {
    console.log(e);
}

//#endregion

//#region ReceivedWebSocketMessages
function ackNewClientConnects(pack) {
    var length = pack.CurrentInterval.ClientDrinks.length;
    for (var i = 0; i < length; i++) {
        drinksVM.drinks.push(pack.CurrentInterval.ClientDrinks[i]);
    }

    //image height:
    var drinksHeight = document.getElementById("drinks").clientHeight;
    console.log("drinksHeight = " + drinksHeight);
    var screenHeightMinNavHeight = document.documentElement.clientHeight - document.getElementById("nav").clientHeight;
    console.log("ScreenLEft  =" + screenHeightMinNavHeight);
    var rows = Math.ceil(length / 4);
    console.log("rows = " + rows);

    if (screenHeightMinNavHeight - drinksHeight > 0) {


        var imageHeight = ((screenHeightMinNavHeight - drinksHeight) / rows - 20);
        console.log("imageHeight = " + imageHeight);
        $(".drink .row img").css("height", imageHeight + "px");

        var maxWidth = $(".drink .row").width();
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
            $(".drink .row img").css("max-width", (maxWidth - 30) +"px");
        }
    }
}
//#endregion

//#region cached values
function checkForCachedValues() {
    if (typeof (Storage) !== "undefined") {
        // Yes! localStorage and sessionStorage support!
        // Some code.....
        if (localStorage.getItem("name") !== undefined) {
            loginVM.name(localStorage.getItem("name"));
        }

        if (localStorage.getItem("serverAdress") !== undefined) {
            loginVM.serverAdress(localStorage.getItem("serverAdress"));
        }

        if (localStorage.getItem("port") !== undefined) {
            loginVM.port(localStorage.getItem("port"));
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