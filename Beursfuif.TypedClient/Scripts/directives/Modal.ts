module beursfuif {
    export class ModalMessages {
        static WRONG_AUTH_TITLE:string = "Verkeerde authenticatie code";
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

        static CONNECTION_LOST_TITLE:string = "Verbinding weggevallen!";
        static CONNECTION_LOST: string = "De verbinding met de server is weggevallen." +
        "<br />Controleer of ..." +
        "<br /><ul>" +
        "<li>... de server nog actief is.</li>" +
        "<li>... je nog op het netwerk zit.</li>";
    }

    export interface IModalScope extends ng.IScope {
        title: string;
        message: string;
        submit: Function;
    }

    export class Modal implements ng.IDirective {
        restrict: string = "EA";
        replace: boolean = true;
        template: string = "<div class='modal fade'>" +
        "<div class='modal-dialog'>" +
        "<div class='modal-content'>" +
        "<div class='modal-header'>" +
        " <button type='button' class='close' data-dismiss='modal' aria-hidden ='true'>&times;</button>" +
        " <h4 class='modal-title'>{{title}}</h4 >" +
        " </div>" +
        "   <div class='modal-body'>" +
        "    </div>" +
        "     <div class='modal-footer' >" +
        "         <button type='button' class='btn btn-primary' data-ng-click='submit()'> Aight </button >" +
        "    </div>" +
        "  </div><!-- /.modal - content-- >" +
        "  </div><!-- /.modal - dialog-- >" +
        "  </div><!-- /.modal-->";
        link(scope: beursfuif.IModalScope, element: any, attrs: any) {
            scope.submit = () => {
                $(element).modal('hide');
            };
            scope.$on(beursfuif.EventNames.OPEN_MODAL, (e: ng.IAngularEvent, ...args: any[]) => {
                if (args[0] && args[1]) {
                    scope.title = args[0];
                    scope.message = args[1];
                    $(".modal-body").html(scope.message);
                    $(element).modal();
                }
            });
        }
    }

    beursfuifModule.directive("bfModal", [() => { return new beursfuif.Modal(); }]);
}
