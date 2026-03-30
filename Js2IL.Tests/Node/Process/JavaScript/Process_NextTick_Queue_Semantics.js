"use strict";

// Verifies current queue behavior for this scheduling pattern.
const order = [];

process.nextTick(() => order.push("nextTick1"));
process.nextTick(() => order.push("nextTick2"));

order.push("sync");

process.nextTick(() => order.push("nextTick3"));

setImmediate(() => {
  order.push("setImmediate");
  console.log("order", order.join(","));
  
  // Expected order for this case: sync, all queued nextTick callbacks, then setImmediate.
  const expected = "sync,nextTick1,nextTick2,nextTick3,setImmediate";
  console.log("correct order", order.join(",") === expected);
});
