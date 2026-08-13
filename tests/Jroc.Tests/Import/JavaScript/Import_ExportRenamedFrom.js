"use strict";

import { renamed, liveCounter, bump } from "./Import_ExportRenamedFrom_Reexport.mjs";

console.log("renamed:", renamed);
console.log("liveCounter0:", liveCounter);
bump();
console.log("liveCounter1:", liveCounter);
