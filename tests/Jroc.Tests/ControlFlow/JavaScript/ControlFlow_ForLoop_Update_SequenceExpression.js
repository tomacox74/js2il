"use strict";

var x = 0, y = 10;

for (var i = 0; i < 3; i = i + 1, x = x + 1, y = y + 2) {
}

console.log(i);
console.log(x);
console.log(y);

var a = 0;
var b = (a = 1, a + 1);
console.log(b);
