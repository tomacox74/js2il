"use strict";

var m = new Map();
m.set("a", 1);
m.set("b", 2);
m.set("c", 3);

// Test keys
var keys = Array.from(m.keys());
console.log(keys.length);
console.log(keys[0]);
console.log(keys[1]);
console.log(keys[2]);

// Test values
var values = Array.from(m.values());
console.log(values.length);
console.log(values[0]);
console.log(values[1]);
console.log(values[2]);
