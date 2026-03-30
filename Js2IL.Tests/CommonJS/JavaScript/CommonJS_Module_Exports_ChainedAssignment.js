"use strict";

// Require a module that uses `exports = module.exports = {...}`.
const lib = require("./CommonJS_Module_Exports_ChainedAssignment_Lib");

console.log("answer:", lib.answer);
console.log("greet:", lib.greet("world"));
