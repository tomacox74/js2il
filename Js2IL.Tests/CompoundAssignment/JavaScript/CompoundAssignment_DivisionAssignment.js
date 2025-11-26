// Test division assignment operator /=
var x = 20;
x /= 4;
console.log('x after 20 /= 4:', x);

// Test repeated division
var value = 64;
for (var i = 1; i <= 3; i++) {
    value /= 2;
    console.log('value after iteration', i, ':', value);
}
console.log('final value:', value);
