"use strict";

import theDefault from "./Import_ReexportAsDefault_Reexport.mjs";
import * as ns from "./Import_ReexportAsDefault_Reexport.mjs";

console.log("default:", theDefault);
console.log("nsDefault:", ns.default);
console.log("hasPayload:", Object.prototype.hasOwnProperty.call(ns, "payload"));
