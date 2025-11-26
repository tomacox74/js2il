// Test remainder/modulo assignment operator %=
var x = 17;
x %= 5;
console.log('x after 17 %= 5:', x);

// Test in a loop
var value = 100;
for (var i = 1; i <= 3; i++) {
    value %= 10;
    console.log('value after iteration', i, ':', value);
}
console.log('final value:', value);
