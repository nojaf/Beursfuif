module beursfuif {
    export class Background implements ng.IDirective {
        restrict: string = "EA";
        link(scope: ng.IScope, element: JQuery, attrs: any) {
            var backgrounds: string[] = ["url('background/wallpaper-1248778.jpg')",
                "url('background/wallpaper-1346213.jpg')",
                "url('background/wallpaper-2143399.jpg')",
                "url('background/wallpaper-2155408.jpg')",
                "url('background/wallpaper-2770643.jpg')",
                "url('background/wallpaper-2837789.jpg')",
                "url('background/wallpaper-2886432.jpg')",
                "url('background/wallpaper-295035.jpg')",
                "url('background/wallpaper-399972.png')",
                "url('background/wallpaper-630648.jpg')",
                "url('background/wallpaper-69099.jpg')",
                "url('background/wallpaper-762606.jpg')"
            ];
            var currentIndex: number = Math.floor(Math.random() * backgrounds.length);
            var $html: JQuery = $("html");
            var $mask: JQuery = $(element);
            var opacity: number = 0.75;

            $html.keyup((event) => {
                if (event.which === 39) {
                    scope.$apply(function () {
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

    beursfuifModule.directive("bfBackground", [() => { return new Background(); }]);
}



