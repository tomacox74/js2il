"use strict";

const a = require("./CommonJS_Require_CircularExportsIdentity_A");

console.log("a.name:", a.name);
console.log("a.seesB:", a.seesB);
console.log("a.bSawA:", a.bSawA);
