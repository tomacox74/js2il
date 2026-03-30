"use strict";

// Issue #167 repro main: import and invoke exported escaping closures.

const lib = require('./CommonJS_Export_ObjectWithClosure_Lib');

console.log('lib.multiplyModuleFactor(3):', lib.multiplyModuleFactor(3));

const calc = lib.createCalculator(10);
console.log('calc.factor:', calc.factor);
console.log('calc.multiply(5):', calc.multiply(5));
