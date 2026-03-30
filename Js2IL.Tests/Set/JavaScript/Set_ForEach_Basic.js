"use strict";

var set = new Set([1, 2]);
var context = { label: "ctx" };
var seen = [];

set.forEach(function (value, key, receiver) {
    seen.push(this.label + ":" + value + ":" + key + ":" + (receiver === set));
}, context);

console.log(seen.length);
console.log(seen[0]);
console.log(seen[1]);

