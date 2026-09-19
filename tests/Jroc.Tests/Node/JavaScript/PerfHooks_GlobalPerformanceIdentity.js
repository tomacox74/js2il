const { performance: modulePerformance } = require("node:perf_hooks");

console.log(typeof performance);
console.log(typeof performance.now);
console.log(performance === modulePerformance);

const before = performance.now();
const after = modulePerformance.now();
console.log(Number.isFinite(before));
console.log(after >= before);

// `now` must not enumerate as an own key (Node exposes it via Performance.prototype).
console.log(Object.keys(performance).length);

performance.now = () => 42;
console.log(performance.now());
