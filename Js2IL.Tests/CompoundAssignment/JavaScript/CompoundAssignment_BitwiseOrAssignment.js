// Test bitwise OR assignment operator |=
// This reproduces the issue found in PrimeJavaScript.js
var x = 5;
x |= 3;
console.log('x after 5 |= 3:', x);

// Test accumulating bits with |=
var allBits = 0;
for (var i = 0; i < 4; i++) {
    allBits |= (1 << i);
    console.log('allBits after iteration', i, ':', allBits);
}
console.log('final allBits:', allBits);

// Test with bit 31 (sign bit)
var value = 0;
value |= (1 << 31);
console.log('value after |= (1 << 31):', value);
