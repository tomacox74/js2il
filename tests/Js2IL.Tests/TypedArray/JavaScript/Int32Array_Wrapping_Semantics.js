"use strict";

// Test that Int32Array uses wrapping semantics, not clamping
const array = new Int32Array(5);

// 2147483648 (2^31) should wrap to -2147483648
array[0] = 2147483648;
console.log(array[0]); // should be -2147483648

// 2147483649 (2^31 + 1) should wrap to -2147483647
array[1] = 2147483649;
console.log(array[1]); // should be -2147483647

// -2147483649 (-(2^31) - 1) should wrap to 2147483647
array[2] = -2147483649;
console.log(array[2]); // should be 2147483647

// Large positive value should wrap
array[3] = 4294967296; // 2^32
console.log(array[3]); // should be 0

array[4] = 4294967297; // 2^32 + 1
console.log(array[4]); // should be 1
