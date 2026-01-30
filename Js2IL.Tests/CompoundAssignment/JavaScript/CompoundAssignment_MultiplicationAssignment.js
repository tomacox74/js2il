"use strict";\r\n\r\n// Test multiplication assignment operator *=
var x = 5;
x *= 3;
console.log('x after 5 *= 3:', x);

// Test repeated multiplication
var product = 2;
for (var i = 1; i <= 4; i++) {
    product *= 2;
    console.log('product after iteration', i, ':', product);
}
console.log('final product:', product);
