"use strict";\r\n\r\n// Test unsigned right shift assignment operator >>>=
var x = 16;
x >>>= 2;
console.log('x after 16 >>>= 2:', x);

// Test with negative numbers (zero-fill shift)
var neg = -16;
neg >>>= 2;
console.log('neg after -16 >>>= 2:', neg);

// Test repeated unsigned right shifts
var value = 64;
for (var i = 0; i < 3; i++) {
    value >>>= 1;
    console.log('value after shift', i + 1, ':', value);
}
console.log('final value:', value);
