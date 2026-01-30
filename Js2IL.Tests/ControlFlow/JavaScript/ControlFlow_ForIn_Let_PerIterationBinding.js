"use strict";\r\n\r\nlet funcs = [];

let obj = { a: 1, b: 2, c: 3 };

for (let k in obj) {
  funcs.push(function () { return k; });
}

for (let f of funcs) {
  console.log(f());
}
