"use strict";

// Test: reading from an Int32Array stored as 'object' in a numeric context.
// This exercises the GetItemAsNumber optimization (peephole fusion of GetItem -> ConvertToNumber).

const arr = new Int32Array(3);
arr[0] = 10;
arr[1] = 20;
arr[2] = 30;

// Unary plus forces numeric coercion of arr[i] result:
// LIRGetItem(arr, 0, result_obj) -> LIRConvertToNumber(result_obj, result_num)
// should be fused to LIRGetItemAsNumber(arr, 0, result_num)
const v0 = +arr[0];
const v1 = +arr[1];
const v2 = +arr[2];
console.log(v0); // 10
console.log(v1); // 20
console.log(v2); // 30
console.log(v0 + v1 + v2); // 60
