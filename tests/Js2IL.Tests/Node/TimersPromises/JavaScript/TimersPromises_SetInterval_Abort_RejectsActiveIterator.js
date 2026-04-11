"use strict";

const timersPromises = require("node:timers/promises");

function createController() {
  const signal = {
    aborted: false,
    reason: null,
    addEventListener: function (type, listener) {
      if (type === "abort") {
        this.listener = listener;
      }
    },
    removeEventListener: function (type, listener) {
      if (type === "abort" && this.listener === listener) {
        this.listener = null;
      }
    }
  };

  return {
    signal: signal,
    abort: function (reason) {
      signal.aborted = true;
      signal.reason = reason;
      if (signal.listener) {
        signal.listener();
      }
    }
  };
}

async function main() {
  const controller = createController();
  const iterator = timersPromises.setInterval(0, "tick", { signal: controller.signal });

  const first = await iterator.next();
  console.log(first.value);

  const second = await iterator.next();
  console.log(second.value);

  controller.abort(new Error("boom-reason"));

  return iterator.next()
    .then(function () {
      console.log("no-reject");
    })
    .catch(function (error) {
      console.log(error.name);
      console.log(error.code);
      console.log(error.message);
      console.log(error.cause.name);
      console.log(error.cause.message);
    });
}

main();
