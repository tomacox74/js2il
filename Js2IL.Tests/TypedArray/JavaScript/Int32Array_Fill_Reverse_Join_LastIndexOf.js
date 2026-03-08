"use strict";

const ints = new Int32Array([1, 2, 3, 2]);
console.log(ints.lastIndexOf(2));
console.log(ints.fill(9, 1, 3).join("|"));
console.log(ints.reverse().toString());
console.log(ints.toString("|"));
console.log(ints.toLocaleString());
