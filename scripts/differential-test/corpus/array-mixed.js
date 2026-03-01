"use strict";
// Risk area: Array methods with length and index interactions

// Array.from and fill
const zeros = new Array(5).fill(0);
console.log(zeros.length);   // 5
console.log(zeros.join(",")); // 0,0,0,0,0

// map: index math in callback
const squares = [1, 2, 3, 4, 5].map(function(v) { return v * v; });
console.log(squares.join(",")); // 1,4,9,16,25

// filter: produces shorter array
const evens = [1, 2, 3, 4, 5, 6].filter(function(v) { return v % 2 === 0; });
console.log(evens.length);    // 3
console.log(evens.join(","));  // 2,4,6

// reduce: fold
const product = [1, 2, 3, 4, 5].reduce(function(acc, v) { return acc * v; }, 1);
console.log(product); // 120

// indexOf / includes
const haystack = [10, 20, 30, 40];
console.log(haystack.indexOf(30));    // 2
console.log(haystack.indexOf(99));    // -1
console.log(haystack.includes(20));   // true
console.log(haystack.includes(99));   // false

// slice
const sliced = haystack.slice(1, 3);
console.log(sliced.join(",")); // 20,30
console.log(haystack.length);  // 4  (original unchanged)

// concat
const combined = [1, 2].concat([3, 4], [5]);
console.log(combined.join(",")); // 1,2,3,4,5
console.log(combined.length);   // 5
