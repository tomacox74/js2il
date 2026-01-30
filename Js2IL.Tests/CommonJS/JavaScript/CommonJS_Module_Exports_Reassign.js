"use strict";\r\n\r\n// Test that module.exports can be reassigned
// This is a key Node.js pattern for replacing the entire exports object

// Add something via exports (which starts as alias to module.exports)
exports.fromExports = "added via exports";

// Check initial state
console.log("module.exports.fromExports:", module.exports.fromExports);

// Reassign module.exports to a new object
module.exports = { newObject: "reassigned module.exports" };

// The new module.exports has the new property
console.log("module.exports.newObject:", module.exports.newObject);

// The new module.exports does NOT have the old property
console.log("module.exports.fromExports after reassign:", module.exports.fromExports);
