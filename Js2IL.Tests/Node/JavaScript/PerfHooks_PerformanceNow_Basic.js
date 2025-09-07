const { performance } = require('perf_hooks');
const t1 = performance.now();
const t2 = performance.now();
console.log(typeof t1 === 'number', typeof t2 === 'number');
