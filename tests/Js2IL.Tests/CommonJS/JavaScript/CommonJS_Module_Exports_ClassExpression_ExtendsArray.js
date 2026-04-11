"use strict";

// Issue #552 repro: a CommonJS module exports a class *expression* that extends Array.
// This should compile without crashing the IR pipeline.

module.exports = class NodeList extends Array {
    constructor(a) {
        super((a && a.length) || 0);
        if (a) {
            for (var idx in a) {
                this[idx] = a[idx];
            }
        }
    }

    item(i) {
        return this[i] || null;
    }
};

console.log("ok");
