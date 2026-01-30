"use strict";\r\n\r\n// Test: Import an object with function properties and call them
// This is the exact repro case from issue #156

const mathUtils = require('./CommonJS_Export_ObjectWithFunctions_Lib');

// Call the imported functions
console.log("add:", mathUtils.add(2, 3));
console.log("multiply:", mathUtils.multiply(4, 5));
console.log("foo:", mathUtils.foo());
