// Minimal test for compound assignment with array element

const arr = new Int32Array(10);

console.log("Initial arr[0]:", arr[0]);

// Test 1: Simple assignment
arr[0] = 5;
console.log("After arr[0] = 5:", arr[0]);

// Test 2: Compound OR assignment
arr[0] |= 2;
console.log("After arr[0] |= 2:", arr[0], "(expected: 7)");

// Test 3: In a loop with variable index
let idx = 1;
arr[idx] = 10;
console.log("After arr[1] = 10:", arr[1]);

arr[idx] |= 4;
console.log("After arr[1] |= 4:", arr[1], "(expected: 14)");

// Test 4: Multiple compound operations
arr[2] = 0;
arr[2] |= 1;
arr[2] |= 4;
arr[2] |= 16;
console.log("After setting bits 0, 2, 4 in arr[2]:", arr[2], "(expected: 21)");
