"use strict";

const timersPromises = require("node:timers/promises");

function createController() {
  const signal = {
    aborted: false,
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
    abort: function () {
      signal.aborted = true;
      if (signal.listener) {
        signal.listener();
      }
    }
  };
}

function logAbort(error) {
  console.log(error.name);
  console.log(error.code);
  console.log(error.message);
}

const timeoutController = createController();
const timeoutPromise = timersPromises.setTimeout(0, "timeout-value", { signal: timeoutController.signal });
timeoutController.abort();

timeoutPromise
  .then(function () {
    console.log("timeout-no-reject");
  })
  .catch(function (error) {
    logAbort(error);
  })
  .then(function () {
    const immediateController = createController();
    immediateController.abort();

    return timersPromises.setImmediate("immediate-value", { signal: immediateController.signal })
      .then(function () {
        console.log("immediate-no-reject");
      })
      .catch(function (error) {
        logAbort(error);
      });
  });
