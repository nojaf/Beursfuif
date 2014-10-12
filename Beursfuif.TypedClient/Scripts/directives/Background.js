var beursfuif;
(function (beursfuif) {
    function Background() {
        return {
            restrict: "EA",
            link: link
        };

        function link(scope, element, attrs) {
            var backgrounds = [
                "url('background/wallpaper-1248778.jpg')",
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
            var currentIndex = Math.floor(Math.random() * backgrounds.length);
            var $html = $("html");
            var $mask = $(element);
            var opacity = 0.75;

            $html.keyup(function (event) {
                if (event.which === 39) {
                    scope.$apply(function () {
                        changeBackground();
                    });

                    event.preventDefault();
                }
            });

            scope.$on("CHANGE_OPACITY", changeOpacity);

            function changeOpacity(e) {
                var args = [];
                for (var _i = 0; _i < (arguments.length - 1); _i++) {
                    args[_i] = arguments[_i + 1];
                }
                opacity = parseFloat(args[0]);
                $mask.animate({ backgroundColor: "rgba(0,0,0," + opacity + ")" }, 500);
            }

            function changeBackground() {
                $mask.animate({ backgroundColor: "#FFF" }, 500, function () {
                    $html.css("background-image", backgrounds[currentIndex]);
                    $mask.animate({ backgroundColor: "rgba(0,0,0," + opacity + ")" }, 500);
                    currentIndex = (currentIndex + 1) % backgrounds.length;
                });
            }

            changeBackground();
        }
    }

    beursfuif.beursfuifModule.directive("bfBackground", [Background]);
})(beursfuif || (beursfuif = {}));
//# sourceMappingURL=Background.js.map
