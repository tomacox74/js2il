"use strict";

var exportsObject = {
    lockedValue: 1
};

exportsObject.readLockedValue = function () {
    return exportsObject.lockedValue;
};

Object.freeze(exportsObject);

module.exports = exportsObject;
