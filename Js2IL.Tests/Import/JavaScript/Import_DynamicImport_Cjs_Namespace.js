"use strict";

import("./Import_DynamicImport_Cjs_Namespace_Lib.cjs").then(nsA => {
    return import("./Import_DynamicImport_Cjs_Namespace_Lib.cjs").then(nsB => {
        console.log("same:", nsA === nsB);
        console.log("keys:", Object.keys(nsA).sort().join(","));
        console.log("defaultSame:", nsA.default === nsA["module.exports"]);
        console.log("value0:", nsA.value);
        nsA.inc();
        console.log("value1:", nsB.value);
        console.log("default1:", nsA.default.value);
    });
}).catch(err => {
    console.error("dynamic import failed:", err && err.message ? err.message : String(err));
});
