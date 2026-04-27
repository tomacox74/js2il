"use strict";

var URL = require("./CommonJS_Require_Captures_Shadowed_Global_URL_Lib");

var descriptor = {
    value: function(href) {
        return new URL("base").resolve(href);
    }
};

console.log(descriptor.value("child"));
