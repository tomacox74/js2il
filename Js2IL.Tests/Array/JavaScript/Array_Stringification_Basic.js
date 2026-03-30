"use strict";

// Array stringification

var arr = [1, 2, 3];
console.log(arr.join());
console.log(arr.toString());
console.log(arr.toLocaleString());

var empty = [];
console.log("[" + empty.toString() + "]");
console.log("[" + empty.toLocaleString() + "]");
