/// <reference path="../app/_references.ts" />
var beursfuif;
(function (beursfuif) {
    var MainCtrl = (function () {
        function MainCtrl($scope, signalrService, $location) {
            this.$scope = $scope;
            this.signalrService = signalrService;
            this.$location = $location;
            if (signalrService.clientInterval) {
                //bind current data
                $scope.drinks = signalrService.clientInterval.ClientDrinks;
                $scope.intervalStart = moment(signalrService.clientInterval.Start).format("HH:mm");
                $scope.intervalEnd = moment(signalrService.clientInterval.End).format("HH:mm");
                $scope.currentTime = moment(signalrService.currentTime).format("HH:mm");

                this.$scope.vm = this;
                this.initScope();
            } else {
                $location.path("/");
            }
        }
        MainCtrl.prototype.initScope = function () {
            var _this = this;
            this.$scope.parseImage = function (imgString) {
                if (imgString) {
                    return "data:image/png;base64," + imgString;
                } else {
                    return "data:image/png;base64,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AAAAAENudENib29sAAAAAABMYmxzYm9vbAAAAAAATmd0dmJvb2wAAAAAAEVtbERib29sAAAAAABJbnRyYm9vbAAAAAAAQmNrZ09iamMAAAABAAAAAAAAUkdCQwAAAAMAAAAAUmQgIGRvdWJAb+AAAAAAAAAAAABHcm4gZG91YkBv4AAAAAAAAAAAAEJsICBkb3ViQG/gAAAAAAAAAAAAQnJkVFVudEYjUmx0AAAAAAAAAAAAAAAAQmxkIFVudEYjUmx0AAAAAAAAAAAAAAAAUnNsdFVudEYjUHhsQFIAAAAAAAAAAAAKdmVjdG9yRGF0YWJvb2wBAAAAAFBnUHNlbnVtAAAAAFBnUHMAAAAAUGdQQwAAAABMZWZ0VW50RiNSbHQAAAAAAAAAAAAAAABUb3AgVW50RiNSbHQAAAAAAAAAAAAAAABTY2wgVW50RiNQcmNAWQAAAAAAAAAAABBjcm9wV2hlblByaW50aW5nYm9vbAAAAAAOY3JvcFJlY3RCb3R0b21sb25nAAAAAAAAAAxjcm9wUmVjdExlZnRsb25nAAAAAAAAAA1jcm9wUmVjdFJpZ2h0bG9uZwAAAAAAAAALY3JvcFJlY3RUb3Bsb25nAAAAAAA4QklNA+0AAAAAABAASAAAAAEAAgBIAAAAAQACOEJJTQQmAAAAAAAOAAAAAAAAAAAAAD+AAAA4QklNBA0AAAAAAAQAAAB4OEJJTQQZAAAAAAAEAAAAHjhCSU0D8wAAAAAACQAAAAAAAAAAAQA4QklNJxAAAAAAAAoAAQAAAAAAAAACOEJJTQP1AAAAAABIAC9mZgABAGxmZgAGAAAAAAABAC9mZgABAKGZmgAGAAAAAAABADIAAAABAFoAAAAGAAAAAAABADUAAAABAC0AAAAGAAAAAAABOEJJTQP4AAAAAABwAAD/////////////////////////////A+gAAAAA/////////////////////////////wPoAAAAAP////////////////////////////8D6AAAAAD/////////////////////////////A+gAADhCSU0EAAAAAAAAAgAAOEJJTQQCAAAAAAACAAA4QklNBDAAAAAAAAEBADhCSU0ELQAAAAAABgABAAAAAzhCSU0ECAAAAAAAEAAAAAEAAAJAAAACQAAAAAA4QklN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IiB4bXA6Q3JlYXRlRGF0ZT0iMjAxMy0wNy0xMVQwMDoxNDo1MCswMjowMCIgeG1wOk1ldGFkYXRhRGF0ZT0iMjAxMy0wNy0xMVQwMDoxNDo1MCswMjowMCIgeG1wOk1vZGlmeURhdGU9IjIwMTMtMDctMTFUMDA6MTQ6NTArMDI6MDAiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6MjE5MTA2NUI4MEU5RTIxMTgwNDNDMDAyRDAxQUIxNTkiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6MjA5MTA2NUI4MEU5RTIxMTgwNDNDMDAyRDAxQUIxNTkiIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDoyMDkxMDY1QjgwRTlFMjExODA0M0MwMDJEMDFBQjE1OSIgZGM6Zm9ybWF0PSJpbWFnZS9qcGVnIiBwaG90b3Nob3A6Q29sb3JNb2RlPSIzIiBwaG90b3Nob3A6SUNDUHJvZmlsZT0ic1JHQiBJRUM2MTk2Ni0yLjEiPiA8eG1wTU06SGlzdG9yeT4gPHJkZjpTZXE+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJjcmVhdGVkIiBzdEV2dDppbnN0YW5jZUlEPSJ4bXAuaWlkOjIwOTEwNjVCODBFOUUyMTE4MDQzQzAwMkQwMUFCMTU5IiBzdEV2dDp3aGVuPSIyMDEzLTA3LTExVDAwOjE0OjUwKzAyOjAwIiBzdEV2dDpzb2Z0d2FyZUFnZW50PSJBZG9iZSBQaG90b3Nob3AgQ1M2IChXaW5kb3dzKSIvPiA8cmRmOmxpIHN0RXZ0OmFjdGlvbj0ic2F2ZWQiIHN0RXZ0Omluc3RhbmNlSUQ9InhtcC5paWQ6MjE5MTA2NUI4MEU5RTIxMTgwNDNDMDAyRDAxQUIxNTkiIHN0RXZ0OndoZW49IjIwMTMtMDctMTFUMDA6MTQ6NTArMDI6MDAiIHN0RXZ0OnNvZnR3YXJlQWdlbnQ9IkFkb2JlIFBob3Rvc2hvcCBDUzYgKFdpbmRvd3MpIiBzdEV2dDpjaGFuZ2VkPSIvIi8+IDwvcmRmOlNlcT4gPC94bXBNTTpIaXN0b3J5PiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8P3hwYWNrZXQgZW5kPSJ3Ij8+/+IMWElDQ19QUk9GSUxFAAEBAAAMSExpbm8CEAAAbW50clJHQiBYWVogB84AAgAJAAYAMQAAYWNzcE1TRlQAAAAASUVDIHNSR0IAAAAAAAAAAAAAAAEAAPbWAAEAAAAA0y1IUCAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAARY3BydAAAAVAAAAAzZGVzYwAAAYQAAABsd3RwdAAAAfAAAAAUYmtwdAAAAgQAAAAUclhZWgAAAhgAAAAUZ1hZWgAAAiwAAAAUYlhZWgAAAkAAAAAUZG1uZAAAAlQAAABwZG1kZAAAAsQAAACIdnVlZAAAA0wAAACGdmlldwAAA9QAAAAkbHVtaQAAA/gAAAAUbWVhcwAABAwAAAAkdGVjaAAABDAAAAAMclRSQwAABDwAAAgMZ1RSQwAABDwAAAgMYlRSQwAABDwAAAgMdGV4dAAAAABDb3B5cmlnaHQgKGMpIDE5OTggSGV3bGV0dC1QYWNrYXJkIENvbXBhbnkAAGRlc2MAAAAAAAAAEnNSR0IgSUVDNjE5NjYtMi4xAAAAAAAAAAAAAAASc1JHQiBJRUM2MTk2Ni0yLjEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFhZWiAAAAAAAADzUQABAAAAARbMWFlaIAAAAAAAAAAAAAAAAAAAAABYWVogAAAAAAAAb6IAADj1AAADkFhZWiAAAAAAAABimQAAt4UAABjaWFlaIAAAAAAAACSgAAAPhAAAts9kZXNjAAAAAAAAABZJRUMgaHR0cDovL3d3dy5p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";
                }
            };

            this.$scope.$on(beursfuif.EventNames.TIME_CHANGED, function (e) {
                _this.updateTime();
            });

            this.$scope.$on(beursfuif.EventNames.OPEN_MODAL, function (e) {
                //we don't really care what the message is.
                //All we know is that something went wrong and the connection is lost
                //So we route back to the login screen
                setTimeout(function () {
                    _this.$location.path("/");
                    _this.$scope.$apply();
                }, 250);
            });

            this.$scope.currentOrder = [];
        };

        MainCtrl.prototype.updateTime = function () {
            console.log(this.signalrService.currentTime);
            this.$scope.currentTime = moment(this.signalrService.currentTime).format("HH:mm");
            this.$scope.$apply();
        };

        MainCtrl.prototype.addItem = function (drinkId, name) {
            //check if item is already a part of the current Order
            var length = this.$scope.currentOrder.length;
            for (var i = 0; i < length; i++) {
                var drinkInOrder = this.$scope.currentOrder[i];
                if (drinkInOrder.DrinkId == drinkId) {
                    drinkInOrder.Count++;
                    return;
                }
            }

            this.$scope.currentOrder.push({
                Count: 1,
                DrinkId: drinkId,
                IntervalId: this.signalrService.clientInterval.Id,
                Name: name
            });
        };

        MainCtrl.prototype.subtractItem = function (drinkId) {
            var length = this.$scope.currentOrder.length;
            for (var i = 0; i < length; i++) {
                var drinkInOrder = this.$scope.currentOrder[i];
                if (drinkInOrder.DrinkId == drinkId) {
                    console.log(drinkInOrder.Count);
                    drinkInOrder.Count--;
                    if (drinkInOrder.Count < 1)
                        this.$scope.currentOrder.splice(i, 1);
                    return;
                }
            }
        };

        MainCtrl.prototype.removeItem = function (drinkId) {
            var length = this.$scope.currentOrder.length;
            for (var i = 0; i < length; i++) {
                var drinkInOrder = this.$scope.currentOrder[i];
                if (drinkInOrder.DrinkId == drinkId) {
                    this.$scope.currentOrder.splice(i, 1);
                    return;
                }
            }
        };

        MainCtrl.prototype.totalOrderPrice = function () {
            var price = 0;
            var length = this.$scope.currentOrder.length;
            for (var i = 0; i < length; i++) {
                var drinkInOrder = this.$scope.currentOrder[i];
                price += drinkInOrder.Count * this.getPrice(drinkInOrder.DrinkId);
            }
            return price;
        };

        MainCtrl.prototype.getPrice = function (drId) {
            var length = this.signalrService.clientInterval.ClientDrinks.length;
            for (var i = 0; i < length; i++) {
                var drink = this.signalrService.clientInterval.ClientDrinks[i];
                if (drink.DrinkId == drId) {
                    return drink.Price;
                }
            }
            return 0;
        };

        MainCtrl.prototype.sendOrder = function () {
            this.signalrService.sendNewOrder(this.$scope.currentOrder);
            this.$scope.currentOrder = [];
        };
        return MainCtrl;
    })();
    beursfuif.MainCtrl = MainCtrl;
})(beursfuif || (beursfuif = {}));
//# sourceMappingURL=MainCtrl.js.map
