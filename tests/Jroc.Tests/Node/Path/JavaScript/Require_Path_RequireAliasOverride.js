"use strict";

const path = require("path");
const requireAlias = require;

requireAlias("path").join = (...parts) => `require-alias:${parts.join("|")}`;

console.log(path.join("a", "b"));
