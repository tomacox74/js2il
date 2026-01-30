"use strict";

const { performance } = require('perf_hooks');
const t1 = performance.now();

let s = 0.0;
let i = 0.0;
for (; i < 10000;) {
	s = s + i;
	i = i + 1;
}

const t2 = performance.now();

let hasNow = true;
try {
	performance.now();
} catch (e) {
	hasNow = false;
}

const dt = t2 - t1;
console.log('hasNow=', hasNow);
console.log('elapsedMsNonNegative=', !(dt < 0.0));
