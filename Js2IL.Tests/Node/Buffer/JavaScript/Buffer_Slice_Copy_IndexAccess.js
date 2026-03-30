"use strict";

// Test slice
var buf1 = Buffer.from([1, 2, 3, 4, 5]);
var sliced = buf1.slice(1, 4);
console.log(sliced.length);
console.log(sliced[0]);
console.log(sliced[1]);
console.log(sliced[2]);

// Test slice with negative indices
var sliced2 = buf1.slice(-3, -1);
console.log(sliced2.length);
console.log(sliced2[0]);
console.log(sliced2[1]);

// Test copy
var buf2 = Buffer.alloc(5);
buf1.copy(buf2, 1, 1, 4);
console.log(buf2[0]);
console.log(buf2[1]);
console.log(buf2[2]);
console.log(buf2[3]);
console.log(buf2[4]);

// Test array-like access
var buf3 = Buffer.alloc(3);
buf3[0] = 65;
buf3[1] = 66;
buf3[2] = 67;
console.log(buf3.toString());

// Test out of bounds access
console.log(buf3[10]);
buf3[10] = 68;
console.log(buf3.length);
