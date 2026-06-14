"use strict";

const order = [];

order.push("sync");

process.nextTick(() => order.push("nextTick1"));

Promise.resolve(0).then(() => {
  order.push("promise1");
  process.nextTick(() => order.push("nextTickFromPromise"));
});

Promise.resolve(0).then(() => order.push("promise2"));

setImmediate(() => {
  order.push("setImmediate");
  setTimeout(() => {
    order.push("setTimeout");
    console.log("order", order.join(","));
    const expected = "sync,nextTick1,promise1,nextTickFromPromise,promise2,setImmediate,setTimeout";
    console.log("correct order", order.join(",") === expected);
  }, 0);
});
