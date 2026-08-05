"use strict";

const { performance } = require("perf_hooks");
performance.now = () => 42;

console.log(performance.now());
