"use strict";

function loadValue() {
    try {
        let value;
        if (value = require("./CommonJS_Require_OptionalChain_Lib")?.value) {
            return value;
        }
    } catch {
    }

    return "missing";
}

console.log(loadValue());
