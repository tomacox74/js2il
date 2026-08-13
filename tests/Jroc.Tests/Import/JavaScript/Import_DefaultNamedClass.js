"use strict";

import Widget from "./Import_DefaultNamedClass_Lib.mjs";

const w = new Widget();
console.log("describe:", w.describe());
console.log("name:", w.name);
console.log("type:", typeof Widget);
