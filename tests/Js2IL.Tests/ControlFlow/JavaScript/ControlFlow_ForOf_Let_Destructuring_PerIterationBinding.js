"use strict";

let funcs = [];

for (let { x } of [{ x: 1 }, { x: 2 }, { x: 3 }]) {
  funcs.push(function () { return x; });
}

for (let f of funcs) {
  console.log(f());
}
