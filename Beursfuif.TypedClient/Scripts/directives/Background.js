var beursfuif;
(function (beursfuif) {
    function Background() {
        return {
            restrict: "EA",
            link: link
        };
        function link(scope, element, attrs) {
            var backgrounds = [
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
                for (var _i = 1; _i < arguments.length; _i++) {
                    args[_i - 1] = arguments[_i];
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