"use strict";

// Test that nextTick callbacks execute in order and before setImmediate
const order = [];

process.nextTick(() => order.push("nextTick1"));
process.nextTick(() => order.push("nextTick2"));

order.push("sync");

process.nextTick(() => order.push("nextTick3"));

setImmediate(() => {
  order.push("setImmediate");
  console.log("order", order.join(","));
  
  // Expected order: All sync code, then all nextTick callbacks in order, then setImmediate
  // sync, nextTick1, nextTick2, nextTick3, setImmediate
  const expected = "sync,nextTick1,nextTick2,nextTick3,setImmediate";
  console.log("correct order", order.join(",") === expected);
});
