"use strict";

import { inner } from "./Import_ExportStarAsNamespace_Reexport.mjs";

console.log("a:", inner.a);
console.log("b:", inner.b);
console.log("default:", inner.default);
console.log("type:", typeof inner);
