"use strict";

exports.callTrim = function (value) {
    return value.trim();
};

exports.createString = function (value) {
    return new String(value);
};

exports.overrideTrim = function (result) {
    String.prototype.trim = function () {
        return result;
    };
};
