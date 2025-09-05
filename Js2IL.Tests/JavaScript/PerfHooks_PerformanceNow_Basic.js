const perf = require('perf_hooks');
const performance = perf.performance;

const t1 = performance.now();
// Do a tiny amount of work.
let s = 0; for (let i = 0; i < 10000; i++) { s += i; }
const t2 = performance.now();

let hasNow = true;
try { performance.now(); } catch (e) { hasNow = false; }
const dt = t2 - t1;
console.log('hasNow=', hasNow);
console.log('elapsedMsNonNegative=', dt >= 0);
