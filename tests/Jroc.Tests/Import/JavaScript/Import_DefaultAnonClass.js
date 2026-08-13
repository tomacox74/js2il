"use strict";

import Cls from "./Import_DefaultAnonClass_Lib.mjs";

const instance = new Cls();
console.log("greet:", instance.greet());
console.log("kind:", instance.kind);
console.log("type:", typeof Cls);
