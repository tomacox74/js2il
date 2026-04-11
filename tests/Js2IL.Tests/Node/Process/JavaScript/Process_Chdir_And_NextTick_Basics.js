"use strict";

const before = process.cwd();
process.chdir(__dirname);
console.log("cwd changed", process.cwd() === __dirname);
process.chdir(before);
console.log("cwd restored", process.cwd() === before);

const order = [];
process.nextTick(() => order.push("tick"));
order.push("sync");
setImmediate(() => {
  order.push("immediate");
  console.log("order", order.join(","));
});
