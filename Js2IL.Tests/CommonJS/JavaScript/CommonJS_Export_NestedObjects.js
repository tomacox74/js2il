"use strict";\r\n\r\n// Test: Import nested literal objects and access fields/methods at various depths

const lib = require('./CommonJS_Export_NestedObjects_Lib');

// Access top-level fields
console.log("name:", lib.name);
console.log("version:", lib.version);

// Access nested object methods
console.log("add:", lib.math.add(10, 5));
console.log("multiply:", lib.math.multiply(4, 3));

// Access nested field
console.log("prefix:", lib.utils.prefix);

// Access nested method
console.log("formatNum:", lib.utils.formatNum(42));
