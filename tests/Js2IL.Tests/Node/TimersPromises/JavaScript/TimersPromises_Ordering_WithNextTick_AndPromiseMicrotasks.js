"use strict";

const timersPromises = require("node:timers/promises");
const order = [];

order.push("sync");

process.nextTick(function () {
  order.push("nextTick");
});

Promise.resolve(0).then(function () {
  order.push("promise");
});

timersPromises.setImmediate("immediate").then(function (value) {
  order.push(value);

  return timersPromises.setTimeout(0, "timeout").then(function (timeoutValue) {
    order.push(timeoutValue);
    console.log("order", order.join(","));
    console.log("correct", order.join(",") === "sync,nextTick,promise,immediate,timeout");
  });
});
