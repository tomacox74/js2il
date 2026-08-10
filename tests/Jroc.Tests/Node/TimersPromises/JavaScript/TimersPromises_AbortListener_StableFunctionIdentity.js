"use strict";

const timersPromises = require("node:timers/promises");

const signal = {
    aborted: false,
    reason: null,
    listener: null,
    addedListener: null,
    addEventListener: function (type, listener) {
        if (type === "abort") {
            this.listener = listener;
            this.addedListener = listener;
            console.log(typeof listener);
            console.log(this.listener === listener);
        }
    },
    removeEventListener: function (type, listener) {
        if (type === "abort") {
            console.log(this.addedListener === listener);
            if (this.listener === listener) {
                this.listener = null;
            }
        }
    }
};

const pending = timersPromises.setTimeout(1000, "value", { signal: signal });
const storedListener = signal.listener;

console.log(typeof storedListener);
signal.aborted = true;
signal.reason = "stopped";
storedListener();

pending.catch(function (error) {
    console.log(error.name);
    console.log(signal.listener === null);
});
