// Deterministic test: construct Date from a known ms value.
// 1970-01-02T10:17:36.789Z => (1 day + 10h + 17m + 36.789s) in ms
// 1 day = 86400000
// 10h = 36000000
// 17m = 1020000
// 36.789s = 36789
const ms = 86400000 + 36000000 + 1020000 + 36789;
const d = new Date(ms);
console.log(d.getTime());
console.log(d.toISOString());
