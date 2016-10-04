module beursfuif {

    function Background(): ng.IDirective {

        return {
            restrict: "EA",
            link:link
        };

        function link(scope: ng.IScope, element: JQuery, attrs: any) {
            var backgrounds: string[] = [
                "url('background/wallhaven-10120.jpg')",
                "url('background/wallhaven-13575.jpg')",
                "url('background/wallhaven-14011.jpg')",
                "url('background/wallhaven-18867.jpg')",
                "url('background/wallhaven-20876.jpg')",
                "url('background/wallhaven-3226.jpg')",
                "url('background/wallhaven-33029.jpg')",
                "url('background/wallhaven-40696.jpg')",
                "url('background/wallhaven-41288.jpg')",
                "url('background/wallhaven-43425.jpg')",
                "url('background/wallhaven-48885.jpg')"
            ];
            var currentIndex: number = Math.floor(Math.random() * backgrounds.length);
            var $html: JQuery = $("html");
            var $mask: JQuery = $(element);
            var opacity: number = 0.75;

            $html.keyup((event) => {
                if (event.which === 39) {
                    scope.$apply(() => {
                        changeBackground();
                    });

                    event.preventDefault();
                }
            });

            scope.$on("CHANGE_OPACITY", changeOpacity);

            function changeOpacity(e: ng.IAngularEvent, ...args: any[]): any {
                opacity = parseFloat(args[0]);
                $mask.animate({ backgroundColor: "rgba(0,0,0," + opacity + ")" }, 500);
            }

            function changeBackground() {
                $mask.animate({ backgroundColor: "#FFF" }, 500, () => {
                    $html.css("background-image", backgrounds[currentIndex]);
                    $mask.animate({ backgroundColor: "rgba(0,0,0," + opacity + ")" }, 500);
                    currentIndex = (currentIndex + 1) % backgrounds.length;
                });
            }

            changeBackground();
        }
    }

    beursfuifModule.directive("bfBackground", [Background]);
}



