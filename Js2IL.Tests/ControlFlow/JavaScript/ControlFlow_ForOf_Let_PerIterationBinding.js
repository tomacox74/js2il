"use strict";\r\n\r\nlet funcs = [];

for (let x of [1, 2, 3]) {
  funcs.push(function () { return x; });
}

for (let f of funcs) {
  console.log(f());
}
