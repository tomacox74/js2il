"use strict";

const timers = require("timers");
const { setTimeout, clearTimeout } = require("node:timers");

console.log(typeof timers.setTimeout);
console.log(typeof timers.clearTimeout);
console.log(typeof setTimeout);
console.log(typeof clearTimeout);

let count = 0;
const timeout = setTimeout(() => {
    count++;
    if (count === 1) {
        timeout.refresh();
    } else {
        clearTimeout(timeout);
        console.log(`flushes: ${count}`);
    }
}, 1);

console.log(`same handle: ${timeout.refresh() === timeout}`);
