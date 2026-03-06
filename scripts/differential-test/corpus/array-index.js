"use strict";
// Risk area: Array .length and index-based arithmetic
// Note: uses var for loop counters to avoid known JS2IL compound-assignment
// issue with for-let iteration variables.

const arr = [10, 20, 30, 40, 50];
console.log(arr.length); // 5

// Sum via index loop
var total = 0;
for (var i = 0; i < arr.length; i++) {
    total += arr[i];
}
console.log(total); // 150

// Reverse traversal
const reversed = [];
for (var ri = arr.length - 1; ri >= 0; ri--) {
    reversed.push(arr[ri]);
}
console.log(reversed.join(",")); // 50,40,30,20,10

// Index arithmetic: every other element
var skipSum = 0;
for (var si = 0; si < arr.length; si += 2) {
    skipSum += arr[si];
}
console.log(skipSum); // 10+30+50 = 90

// Nested index math
const matrix = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];
var diagSum = 0;
for (var di = 0; di < matrix.length; di++) {
    diagSum += matrix[di][di];
}
console.log(diagSum); // 1+5+9 = 15

// Length changes
const dyn = [1, 2, 3];
dyn.push(4);
console.log(dyn.length); // 4
dyn.pop();
console.log(dyn.length); // 3
console.log(dyn[dyn.length - 1]); // 3
