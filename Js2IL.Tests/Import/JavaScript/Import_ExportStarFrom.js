"use strict";

import * as ns from "./Import_ExportStarFrom_LibB.mjs";

console.log("own:", ns.own);
console.log("hasInherited:", Object.prototype.hasOwnProperty.call(ns, "inherited"));
console.log("hasDefault:", Object.prototype.hasOwnProperty.call(ns, "default"));
