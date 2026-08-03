"use strict";

const path = require("path");

module.Require("path").join = (...parts) => `module-require:${parts.join("|")}`;

console.log(path.join("a", "b"));
