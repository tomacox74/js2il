"use strict";

var map = new Map([
    ["a", 1],
    ["b", 2]
]);

var context = { prefix: "ctx" };
var seen = [];

map.forEach(function (value, key, receiver) {
    seen.push(this.prefix + ":" + key + "=" + value + ":" + (receiver === map));
}, context);

console.log(seen.length);
console.log(seen[0]);
console.log(seen[1]);

