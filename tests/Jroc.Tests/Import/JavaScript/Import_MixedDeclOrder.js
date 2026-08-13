"use strict";

export * from "./Import_MixedDeclOrder_A.mjs";
import { fromB } from "./Import_MixedDeclOrder_B.mjs";

console.log("entry body:", fromB);
