"use strict";\r\n\r\n// Test: Import a class constructor from another module and instantiate it
// This tests cross-module class constructor exports + `new` in the importing module

const Calculator = require('./CommonJS_Export_Class_Lib');
const calc = new Calculator();

// Call methods on the created instance
console.log("add:", calc.add(2, 3));
console.log("multiply:", calc.multiply(4, 5));
