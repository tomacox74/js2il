"use strict";

if (false) {
    require("node:bla");
}

try {
    require("node:bla");
} catch (error) {
    console.log(error.name);
    console.log(error.code);
    console.log(error.message);
}
