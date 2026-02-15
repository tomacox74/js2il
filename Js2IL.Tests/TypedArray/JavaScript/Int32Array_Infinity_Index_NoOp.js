"use strict";

// Test that Infinity index does not write to element 0
const array = new Int32Array(5);
array[0] = 100;
array[Infinity] = 999;
console.log(array[0]); // should be 100, not 999

array[1] = 200;
array[-Infinity] = 888;
console.log(array[1]); // should be 200, not 888
