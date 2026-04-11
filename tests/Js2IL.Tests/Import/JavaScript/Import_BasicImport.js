"use strict";
// Test basic dynamic import() with a literal specifier
// Should return a Promise that resolves to the module exports

console.log("Testing dynamic import...");

import("node:path").then(module => {
    console.log("Import succeeded.");
    console.log("typeof module:", typeof module);
    console.log("module loaded:", module != null);
}).catch(err => {
    console.error("Import failed:", err);
});
