"use strict";

// Regression/coverage for stable typing of captured const numbers.
// The global const is captured by the nested function, so it must be stored as a scope field.
const NOW_UNITS_PER_SECOND = 1000;

function run() {
	return NOW_UNITS_PER_SECOND + 23;
}

console.log(run());
