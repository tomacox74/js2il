"use strict";

const path = require("path");

path.join++;

try {
    path.join("a", "b");
} catch (error) {
    console.log(error.name);
}
