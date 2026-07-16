"use strict";

const { setTimeout, clearTimeout } = require("node:timers");

let fired = false;
const timeout = setTimeout(() => {
    fired = true;
}, 1);

clearTimeout(timeout);
console.log(`same handle: ${timeout.refresh() === timeout}`);

setTimeout(() => {
    console.log(`canceled timeout fired: ${fired}`);
}, 2);
