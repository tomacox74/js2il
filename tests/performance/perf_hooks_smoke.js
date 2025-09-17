"use strict";
const { performance } = require('perf_hooks');
console.log('perf.now #1:', performance.now());
setTimeout(() => {
  console.log('perf.now #2:', performance.now());
}, 10);
