"use strict";

const timersPromises = require("node:timers/promises");

var options = {};
Object.defineProperty(options, "signal", {
  get: function () {
    throw new Error("boom-signal");
  }
});

const promise = timersPromises.setTimeout(0, "value", options);

console.log(typeof promise.then);

promise
  .then(function () {
    console.log("signal-getter-no-reject");
  })
  .catch(function (error) {
    console.log(error.name);
    console.log(error.message);
  })
  .then(function () {
    var signal = {
      addEventListener: function () { }
    };
    Object.defineProperty(signal, "aborted", {
      get: function () {
        throw new Error("boom-aborted");
      }
    });

    try {
      timersPromises.setTimeout(0, "value", {
        signal: signal
      });

      console.log("aborted-getter-no-throw");
    } catch (error) {
      console.log(error.name);
      console.log(error.message);
    }
  });
