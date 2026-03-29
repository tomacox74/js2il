"use strict";

var set = new Set([1, 2]);
var keys = Array.from(set.keys());
var values = Array.from(set.values());
var entries = Array.from(set.entries());

console.log(keys.length);
console.log(keys[0]);
console.log(keys[1]);
console.log(values.length);
console.log(values[0]);
console.log(values[1]);
console.log(entries.length);
console.log(entries[0][0]);
console.log(entries[0][1]);
console.log(entries[1][0]);
console.log(entries[1][1]);

