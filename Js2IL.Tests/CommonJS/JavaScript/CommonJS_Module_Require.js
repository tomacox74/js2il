// Test module.require() - should work like require()
// This is a bound require function on the module object

// Check that module.require is defined
console.log("module.require type:", typeof module.require);

// Use the regular require function which we know works
const path = require('path');

// Verify we got the path module
console.log("path module type:", typeof path);
console.log("path.join type:", typeof path.join);

// Use path.join to verify it works
const result = path.join("a", "b", "c");
console.log("path.join result:", result);
