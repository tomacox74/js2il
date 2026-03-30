"use strict";

var m = new Map();
m.set("key1", "value1");
console.log(m.get("key1"));
m.set("key1", "value2");
console.log(m.get("key1"));
console.log(m.size);
