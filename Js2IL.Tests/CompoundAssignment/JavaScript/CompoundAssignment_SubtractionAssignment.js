"use strict";\r\n\r\n// Test subtraction assignment operator -=
var x = 10;
x -= 3;
console.log('x after 10 -= 3:', x);

// Test accumulation
var sum = 100;
for (var i = 1; i <= 5; i++) {
    sum -= i;
    console.log('sum after iteration', i, ':', sum);
}
console.log('final sum:', sum);
