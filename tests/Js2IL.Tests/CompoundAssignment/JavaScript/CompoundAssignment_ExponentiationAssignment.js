"use strict";

// Test exponentiation assignment operator **=
var x = 2;
x **= 3;
console.log('x after 2 **= 3:', x);

// Test repeated exponentiation
var value = 2;
for (var i = 1; i <= 4; i++) {
    value **= 2;
    console.log('value after iteration', i, ':', value);
}
console.log('final value:', value);
