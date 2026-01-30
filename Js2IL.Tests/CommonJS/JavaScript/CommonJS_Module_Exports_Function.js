"use strict";\r\n\r\n// Test module.exports = function pattern
// Common pattern to export a single function

function greet(name) {
    return "Hello, " + name + "!";
}

module.exports = greet;

// The module.exports is now a function
console.log("typeof module.exports:", typeof module.exports);

// Call the exported function
var fn = module.exports;
console.log("result:", fn("World"));
