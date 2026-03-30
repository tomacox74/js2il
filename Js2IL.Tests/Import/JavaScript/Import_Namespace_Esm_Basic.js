"use strict";

import * as ns from "./Import_Namespace_Esm_Basic_Lib.mjs";

console.log("keys:", Object.keys(ns).sort().join(","));
console.log("enum.default:", Object.prototype.propertyIsEnumerable.call(ns, "default"));
console.log("enum.inc:", Object.prototype.propertyIsEnumerable.call(ns, "inc"));
console.log("enum.x:", Object.prototype.propertyIsEnumerable.call(ns, "x"));
console.log("x0:", ns.x);
console.log("def0:", ns.default());
ns.inc();
console.log("x1:", ns.x);
console.log("def1:", ns.default());
