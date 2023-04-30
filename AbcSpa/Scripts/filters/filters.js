"use strict";

angular.module("abcadmin")

    .filter("slice", function () {
        return function (arr, start, end) {
            if (arr != null)
                return arr.slice(start, end);
        };
    })

;