"use strict";

import("./Import_DynamicImport_Cjs_FrozenNamespaceStable_Lib.cjs").then(nsA => {
    return import("./Import_DynamicImport_Cjs_FrozenNamespaceStable_Lib.cjs").then(nsB => {
        console.log("same:", nsA === nsB);
        console.log("defaultSame:", nsA.default === nsB.default);
        console.log("kind:", nsA.kind);
        console.log("defaultKind:", nsA.default.kind);
    });
}).catch(err => {
    console.error("dynamic import failed:", err && err.message ? err.message : String(err));
});
