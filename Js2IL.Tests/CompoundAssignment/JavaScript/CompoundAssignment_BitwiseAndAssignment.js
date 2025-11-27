// Test bitwise AND assignment operator &=
var x = 7;
x &= 3;
console.log('x after 7 &= 3:', x);

// Test accumulating with &= (clearing bits)
var value = 15;
for (var i = 0; i < 4; i++) {
    value &= (14 - i);  // 14, 13, 12, 11
    console.log('value after iteration', i, ':', value);
}
console.log('final value:', value);
