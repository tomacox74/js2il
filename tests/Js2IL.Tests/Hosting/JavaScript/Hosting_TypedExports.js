"use strict";

class Counter {
    constructor(start) {
        this.value = start;
    }

    add(delta) {
        this.value = this.value + delta;
        return this.value;
    }

    getValue() {
        return this.value;
    }
}

function add(x, y) {
    return x + y;
}

function fail() {
    throw new Error("boom");
}

function createCounter(start) {
    return new Counter(start);
}

function readMutableValue() {
    return module.exports.mutableValue;
}

function readExport(name) {
    return module.exports[name];
}

const version = "1.2.3";
let mutableValue = 0;

module.exports = {
    version,
    mutableValue,
    readMutableValue,
    readExport,
    add,
    fail,
    Counter,
    createCounter
};
