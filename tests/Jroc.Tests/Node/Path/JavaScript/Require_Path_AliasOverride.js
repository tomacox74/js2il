"use strict";

const path = require("path");
const alias = path;

alias.join = (...parts) => `aliased:${parts.join("|")}`;

console.log(path.join("a", "b"));
