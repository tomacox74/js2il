"use strict";

function rootIncrement(value) {
    return value + 1;
}

rootIncrement.kind = "root-function";
module.exports = rootIncrement;
