"use strict";

// Minimal repro for do/while correctness with numeric loop-carried variables.
// PrimeJavaScript's large-step fast path uses a do/while with `wordOffset += step` and a `<=` test.

let wordOffset = 0;
const step = 17;
const range_stop_word = 100;
let count = 0;

do {
	count++;
	wordOffset += step;
} while (wordOffset <= range_stop_word);

console.log("count", count);
console.log("final", wordOffset);
