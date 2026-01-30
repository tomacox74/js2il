"use strict";\r\n\r\n// ECMA-262: https://tc39.es/ecma262/#sec-parseint-string-radix
// Minimal conformance checks for js2il runtime.

console.log(parseInt("  123"));
console.log(parseInt("\t\n-123"));
console.log(parseInt("15px", 10));
console.log(parseInt("0x10"));
console.log(parseInt("0x10", 0));
console.log(parseInt("0x10", 16));
console.log(parseInt("11", 2));
console.log(parseInt("11", 8));
console.log(parseInt("xyz"));
console.log(parseInt(""));
console.log(parseInt("10", 1));
