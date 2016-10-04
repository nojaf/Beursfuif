var beursfuif;
(function (beursfuif) {
    function Modal() {
        return {
            restrict: "EA",
            replace: true,
            template: "<div class='modal fade'>" +
                "<div class='modal-dialog'>" +
                "<div class='modal-content'>" +
                "<div class='modal-header'>" +
                " <button type='button' class='close' data-dismiss='modal' aria-hidden ='true'>&times;</button>" +
                " <h4 class='modal-title'>{{title}}</h4 >" +
                " </div>" +
                "   <div class='modal-body'>" +
                "    </div>" +
                "<div class=\"text-center\"><img src=\"/images/error.jpg\" id=\"error-image\"></div>" +
                "     <div class='modal-footer' >" +
                "         <button type='button' class='btn btn-primary' data-ng-click='submit()'> Aight </button >" +
                "    </div>" +
                "  </div><!-- /.modal - content-- >" +
                "  </div><!-- /.modal - dialog-- >" +
                "  </div><!-- /.modal-->",
            link: link
        };
        function link(scope, element, attrs) {
            scope.submit = function () {
                $(element).modal('hide');
            };
            scope.$on(beursfuif.EventNames.OPEN_MODAL, function (e) {
                var args = [];
                for (var _i = 1; _i < arguments.length; _i++) {
                    args[_i - 1] = arguments[_i];
                }
                if (args[0] && args[1]) {
                    scope.title = args[0];
                    scope.message = args[1];
                    $(".modal-body").html(scope.message);
                    $(element).modal({ backdrop: false });
                }
            });
        }
    }
    beursfuif.beursfuifModule.directive("bfModal", [Modal]);
})(beursfuif || (beursfuif = {}));
//# sourceMappingURL=Modal.js.map