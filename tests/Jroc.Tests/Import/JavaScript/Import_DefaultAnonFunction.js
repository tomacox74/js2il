"use strict";

import fn, { label } from "./Import_DefaultAnonFunction_Lib.mjs";

console.log("label:", label);
console.log("result:", fn());
console.log("type:", typeof fn);
