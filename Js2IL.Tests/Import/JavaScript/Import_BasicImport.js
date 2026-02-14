"use strict";
// Test basic dynamic import() with a literal specifier
// Should return a Promise that resolves to the module exports

async function test() {
    console.log("Testing dynamic import...");
    
    // Import a dependency module (without leading ./ like in the naming)
    const module = await import("./Import_BasicImport_Dep");
    
    console.log("Import successful!");
    console.log("typeof module:", typeof module);
    console.log("module.message:", module.message);
    console.log("module.getValue():", module.getValue());
}

test().catch(err => {
    console.error("Import failed:", err);
});
