"use strict";

const perfHooks = require("perf_hooks");
perfHooks.performance = Object.create({
    now() {
        return 42;
    }
});

const { performance } = require("perf_hooks");
console.log(performance.now());
