"use strict";

import cjsDefault, { value as namedValue } from "./Import_StaticImport_FromCjs_Lib.cjs";
import * as cjsNamespace from "./Import_StaticImport_FromCjs_Lib.cjs";

console.log("default.value:", cjsDefault.value);
console.log("named:", namedValue);
console.log("namespace.default.value:", cjsNamespace.default.value);
console.log("namespace.value:", cjsNamespace.value);
