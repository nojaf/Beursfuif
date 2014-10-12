module beursfuif {
    export class ModalMessages {
        static WRONG_AUTH_TITLE: string = "Verkeerde authenticatie code";
        static WRONG_AUTH: string = "Je authenticatie code komt niet overeen met die van de server. " +
        "<br />Dit wil zeggen dat je niet de juiste prijzen hebt." +
        "<br />De connectie met de server wordt verbroken." +
        "<br />Gelieve opnieuw aan te melden.";

        static YOU_GOT_KICKED_TITLE: string = "You got kicked";
        static YOU_GOT_KICKED: string = "De server heeft de verbinding opzettelijk verbroken." +
        "<br />Dit kan zijn omdat ..." +
        "<br><ul>" +
        "<li>... je niet meer met de correcte prijzen bezig was.</li>" +
        "<li>... de server is gestopt en ondervindt problemen.</li>" +
        "<li>... de server werd gepauseerd.</li>" +
        "<li>...de persoon aan de serverkant doesn't like you.</li>" +
        "</ul>" +
        "<br />Gelieve opnieuw aan te melden of even te wachten.";

        static CONNECTION_LOST_TITLE: string = "Verbinding weggevallen!";
        static CONNECTION_LOST: string = "De verbinding met de server is weggevallen." +
        "<br />Controleer of ..." +
        "<br /><ul>" +
        "<li>... de server nog actief is.</li>" +
        "<li>... je nog op het netwerk zit.</li>";
    }

} 