"use strict";\r\n\r\nclass Counter {
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

const version = "1.2.3";

module.exports = {
    version,
    add,
    fail,
    Counter,
    createCounter
};
