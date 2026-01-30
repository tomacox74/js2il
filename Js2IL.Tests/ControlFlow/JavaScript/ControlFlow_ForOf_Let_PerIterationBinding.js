"use strict";

let funcs = [];

for (let x of [1, 2, 3]) {
  funcs.push(function () { return x; });
}

for (let f of funcs) {
  console.log(f());
}
