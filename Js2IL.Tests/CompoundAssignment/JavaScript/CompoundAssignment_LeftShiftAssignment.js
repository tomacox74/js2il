"use strict";\r\n\r\n// Test left shift assignment operator <<=
var x = 1;
x <<= 3;
console.log('x after 1 <<= 3:', x);

// Test repeated left shifts
var value = 1;
for (var i = 0; i < 5; i++) {
    value <<= 1;
    console.log('value after shift', i + 1, ':', value);
}
console.log('final value:', value);
