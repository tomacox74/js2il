"use strict";

// Test that fractional indices do not write to element 0 or 1
const array = new Int32Array(5);
array[0] = 100;
array[1] = 200;
array[1.5] = 999;
array[1.9] = 888;
console.log(array[0]); // should be 100
console.log(array[1]); // should be 200, not 999 or 888
console.log(array[2]); // should be 0
