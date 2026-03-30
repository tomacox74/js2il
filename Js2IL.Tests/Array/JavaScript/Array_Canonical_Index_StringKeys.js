"use strict";

var a = [10, 20, 30];

console.log(a["length"]);
console.log(a["0"]);
console.log(a["01"]);

a["01"] = 42;
console.log(a["01"]);
