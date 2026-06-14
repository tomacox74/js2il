"use strict";

const order = [];

setImmediate(() => order.push("setImmediate"));
process.nextTick(() => order.push("nextTick"));

setTimeout(() => {
  order.push("setTimeout");
  console.log("order", order.join(","));
  console.log("nextTick first", order[0] === "nextTick");
}, 0);
