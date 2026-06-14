"use strict";

var set = new Set();
set.add(1);
set.add(2);
set.add(2);

console.log(set.size);
console.log(set.delete(2));
console.log(set.has(2));
console.log(set.size);
set.clear();
console.log(set.size);

