"use strict";

exports.name = "a";

const b = require("./CommonJS_Require_CircularExportsIdentity_B");
exports.seesB = b.name;
exports.bSawA = b.sawA;
