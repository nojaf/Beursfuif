var beursfuif;
(function (beursfuif) {
    var ModalMessages = (function () {
        function ModalMessages() {
        }
        ModalMessages.WRONG_AUTH_TITLE = "Verkeerde authenticatie code";
        ModalMessages.WRONG_AUTH = "Je authenticatie code komt niet overeen met die van de server. " +
            "<br />Dit wil zeggen dat je niet de juiste prijzen hebt." +
            "<br />De connectie met de server wordt verbroken." +
            "<br />Gelieve opnieuw aan te melden.";
        ModalMessages.YOU_GOT_KICKED_TITLE = "You got kicked";
        ModalMessages.YOU_GOT_KICKED = "De server heeft de verbinding opzettelijk verbroken." +
            "<br />Dit kan zijn omdat ..." +
            "<br><ul>" +
            "<li>... je niet meer met de correcte prijzen bezig was.</li>" +
            "<li>... de server is gestopt en ondervindt problemen.</li>" +
            "<li>... de server werd gepauseerd.</li>" +
            "<li>...de persoon aan de serverkant doesn't like you.</li>" +
            "</ul>" +
            "<br />Gelieve opnieuw aan te melden of even te wachten.";
        ModalMessages.CONNECTION_LOST_TITLE = "Verbinding weggevallen!";
        ModalMessages.CONNECTION_LOST = "De verbinding met de server is weggevallen." +
            "<br />Controleer of ..." +
            "<br /><ul>" +
            "<li>... de server nog actief is.</li>" +
            "<li>... je nog op het netwerk zit.</li>";
        return ModalMessages;
    }());
    beursfuif.ModalMessages = ModalMessages;
})(beursfuif || (beursfuif = {}));
//# sourceMappingURL=modalMessages.js.map