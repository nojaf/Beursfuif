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
    var initialPackage = new Package({ ClientName: loginVM.name() , MessageId:1});
    webSocket.send(JSON.stringify(initialPackage));

    //TODO: set Timer for ack
}

function webSocketCloseHandler(e) {
    console.log(e);
}

function webSocketMessageHandler(e) {
    var pack = new Package(JSON.parse(e.data));
    console.log("msgId = " +pack.MessageId)
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
    for (var i = 0; i < length*5; i++) {
        drinksVM.drinks.push(pack.CurrentInterval.ClientDrinks[i%3]);
    }

    //image height:
    var drinksHeight = document.getElementById("drinks").clientHeight;
    console.log("drinksHeight = "+ drinksHeight)
    var screenHeightMinNavHeight = document.documentElement.clientHeight - document.getElementById("nav").clientHeight;
    console.log("ScreenLEft  =" + screenHeightMinNavHeight);
    var rows = Math.ceil(length * 5/ 4);
    var imageHeight = $(".drink .row img").height();
    if (screenHeightMinNavHeight - drinksHeight > 0) {


        imageHeight += ((screenHeightMinNavHeight - drinksHeight) / rows - 20);
        console.log("imageHeight = " + imageHeight);
        $(".drink .row img").css("height", imageHeight + "px");

        var maxWidth = $(".drink .row").width();
        console.log(maxWidth);
        console.log("image width = " + $(".drink .row img").css("width"));
        if ($(".drink .row img").css("width") - 30 > maxWidth) {
            $(".drink .row img").css("height", "auto")
            $(".drink .row img").css("width", (maxWidth - 30) + "px");
        }
    }
}
//#endregion