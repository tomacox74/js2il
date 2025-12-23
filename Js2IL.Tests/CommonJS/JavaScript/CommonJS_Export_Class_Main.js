// Test: Import a class instance from another module
// This tests cross-module class instance exports

const calc = require('./CommonJS_Export_Class_Lib');

// Call methods on the imported instance
console.log("add:", calc.add(2, 3));
console.log("multiply:", calc.multiply(4, 5));
