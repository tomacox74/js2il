"use strict";

const a = require("./CommonJS_Require_CircularExportsIdentity_A");

exports.name = "b";
exports.sawA = a.name;
