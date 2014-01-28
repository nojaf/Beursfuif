/// <reference path="../app/_references.ts" />
var beursfuif;
(function (beursfuif) {
    var ModalMessages = (function () {
        function ModalMessages() {
        }
        ModalMessages.WRONG_AUTH_TITLE = "Verkeerde authenticatie code";
        ModalMessages.WRONG_AUTH = "Je authenticatie code komt niet overeen met die van de server. " + "<br />Dit wil zeggen dat je niet de juiste prijzen hebt." + "<br />De connectie met de server wordt verbroken." + "<br />Gelieve opnieuw aan te melden.";

        ModalMessages.YOU_GOT_KICKED_TITLE = "You got kicked";
        ModalMessages.YOU_GOT_KICKED = "De server heeft de verbinding opzettelijk verbroken." + "<br />Dit kan zijn omdat ..." + "<br><ul>" + "<li>... je niet meer met de correcte prijzen bezig was.</li>" + "<li>... de server is gestopt en ondervindt problemen.</li>" + "<li>... de server werd gepauseerd.</li>" + "<li>...de persoon aan de serverkant doesn't like you.</li>" + "</ul>" + "<br />Gelieve opnieuw aan te melden of even te wachten.";

        ModalMessages.CONNECTION_LOST_TITLE = "Verbinding weggevallen!";
        ModalMessages.CONNECTION_LOST = "De verbinding met de server is weggevallen." + "<br />Controleer of ..." + "<br /><ul>" + "<li>... de server nog actief is.</li>" + "<li>... je nog op het netwerk zit.</li>";
        return ModalMessages;
    })();
    beursfuif.ModalMessages = ModalMessages;

    var Modal = (function () {
        function Modal() {
            this.restrict = "EA";
            this.replace = true;
            this.template = "<div class='modal fade'>" + "<div class='modal-dialog'>" + "<div class='modal-content'>" + "<div class='modal-header'>" + " <button type='button' class='close' data-dismiss='modal' aria-hidden ='true'>&times;</button>" + " <h4 class='modal-title'>{{title}}</h4 >" + " </div>" + "   <div class='modal-body'>" + "    </div>" + "     <div class='modal-footer' >" + "         <button type='button' class='btn btn-primary' data-ng-click='submit()'> Aight </button >" + "    </div>" + "  </div><!-- /.modal - content-- >" + "  </div><!-- /.modal - dialog-- >" + "  </div><!-- /.modal-->";
        }
        Modal.prototype.link = function (scope, element, attrs) {
            scope.submit = function () {
                $(element).modal('hide');
            };
            scope.$on(beursfuif.EventNames.OPEN_MODAL, function (e) {
                var args = [];
                for (var _i = 0; _i < (arguments.length - 1); _i++) {
                    args[_i] = arguments[_i + 1];
                }
                if (args[0] && args[1]) {
                    scope.title = args[0];
                    scope.message = args[1];
                    $(".modal-body").html(scope.message);
                    $(element).modal();
                }
            });
        };
        return Modal;
    })();
    beursfuif.Modal = Modal;
})(beursfuif || (beursfuif = {}));
//# sourceMappingURL=Modal.js.map
