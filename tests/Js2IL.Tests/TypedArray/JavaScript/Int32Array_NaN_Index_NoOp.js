"use strict";

// Test that NaN index does not write to element 0
const array = new Int32Array(5);
array[0] = 100;
array[NaN] = 999;
console.log(array[0]); // should be 100, not 999
