"use strict";

const path = require("path");

function patchPath(
    ignored = (path.join = (...parts) => `default:${parts.join("|")}`)
) {
}

patchPath();
console.log(path.join("a", "b"));
