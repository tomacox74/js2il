"use strict";

exports.callTrim = function (value) {
    return value.trim();
};

exports.overrideTrim = function (result) {
    String.prototype.trim = function () {
        return result;
    };
};
