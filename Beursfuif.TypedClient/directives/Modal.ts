module beursfuif {
    export interface IModalScope extends ng.IScope {
        title: string;
        message: string;
        submit: Function;
    }
}

function Modal() {
    return {
        restrict: "EA",
        link: function (scope: beursfuif.IModalScope, element: any, attrs: any) {
            //setTimeout(() => {
            //    console.log(scope);
            //    scope.title = "Testtitle";
            //    scope.message = "Ipsum";
            //    scope.$apply();
            //    $(element).modal();
            //}, 1000);

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
        },
        template: "<div class='modal fade'>" +
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
        "  </div><!-- /.modal-->",
        replace: true
    };
} 