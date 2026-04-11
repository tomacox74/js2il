"use strict";

const esm = require("./Import_RequireEsmModule_Lib.mjs");

console.log("default:", esm.default);
console.log("named:", esm.named);
console.log("sum:", esm.sum(2, 3));
