"use strict";\r\n\r\n// Mutating Array operations

var d = [1, 2, 3];
console.log(d.shift());
console.log(d.join());
console.log(d.unshift(9, 8));
console.log(d.join());

var e = [1, 2, 3];
console.log(e.reverse().join());

var j = [0, 0, 0, 0];
j.fill(7, 1, 3);
console.log(j.join());

var k = [1, 2, 3, 4, 5];
k.copyWithin(0, 3);
console.log(k.join());
