"use strict";\r\n\r\n// Test compound bitwise OR assignment on array elements (dynamic indexed access)
// This triggers the dynamic fallback path that uses CoerceToInt32

var arr = [1, 2, 4, 8];

// Test basic array element compound assignment
arr[0] |= 16;
console.log('arr[0] after 1 |= 16:', arr[0]);

// Test in a loop (similar to PrimeJavaScript.js pattern)
for (var i = 0; i < arr.length; i++) {
    arr[i] |= (1 << i);
    console.log('arr[' + i + '] after |= (1 << ' + i + '):', arr[i]);
}

// Test with computed index
var index = 2;
arr[index] |= 32;
console.log('arr[2] after |= 32:', arr[2]);

console.log('Final array:', arr);
