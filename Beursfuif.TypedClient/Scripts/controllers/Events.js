var beursfuif;
(function (beursfuif) {
    var EventNames = (function () {
        function EventNames() {
        }
        EventNames.CONNECTION_CHANGED = "CONNECTION_CHANGED";
        EventNames.CHANGE_OPACITY = "CHANGE_OPACITY";
        EventNames.TIME_CHANGED = "TIME_CHANGED";
        EventNames.OPEN_MODAL = "OPEN_MODAL";
        EventNames.SHOW_TOAST = "SHOW_TOAST";
        EventNames.INTERVAL_UPDATE = "INTERVAL_UPDATE";
        EventNames.DRINK_AVAILABLE_CHANGED = "DRINK_AVAILABLE_CHANGED";
        return EventNames;
    }());
    beursfuif.EventNames = EventNames;
})(beursfuif || (beursfuif = {}));
//# sourceMappingURL=Events.js.map