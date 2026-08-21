"use strict";

exports.callTrim = function (value) {
    let result;
    for (let index = 0; index < 3; index++) {
        result = value.trim();
        result = value.trim();
    }
    return result;
};

exports.createString = function (value) {
    return new String(value);
};

exports.overrideTrim = function (result) {
    String.prototype.trim = function () {
        return result;
    };
};
