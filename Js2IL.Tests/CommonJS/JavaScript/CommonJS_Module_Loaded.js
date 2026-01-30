"use strict";\r\n\r\n// Test module.loaded property
// module.loaded is false while the module is loading, true after

// During module execution, loaded should be false
console.log("module.loaded during execution:", module.loaded);

// We can't test the 'true' case directly in the same module
// because we're still executing - but we can verify the property exists
console.log("module.loaded type:", typeof module.loaded);
console.log("module.loaded is boolean:", typeof module.loaded === "boolean");
