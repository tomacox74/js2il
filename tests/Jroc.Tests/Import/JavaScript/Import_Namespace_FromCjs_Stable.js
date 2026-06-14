"use strict";

import { ns as nsA } from "./Import_Namespace_FromCjs_Stable_A.mjs";
import { ns as nsB } from "./Import_Namespace_FromCjs_Stable_B.mjs";

console.log("same:", nsA === nsB);
console.log("keys:", Object.keys(nsA).sort().join(","));
console.log("defaultIsModuleExports:", nsA.default === nsA["module.exports"]);
console.log("x0:", nsA.x);
nsA.inc();
console.log("x1:", nsB.x);
