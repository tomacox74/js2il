// Test bitwise XOR assignment operator ^=
var x = 5;
x ^= 3;
console.log('x after 5 ^= 3:', x);

// Test toggling bits with ^=
var value = 0;
for (var i = 0; i < 4; i++) {
    value ^= (1 << i);
    console.log('value after iteration', i, ':', value);
}
// Toggle them all off again
for (var i = 0; i < 4; i++) {
    value ^= (1 << i);
}
console.log('final value after toggle off:', value);
