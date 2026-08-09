"use strict";

function ordinary(left, right) {
    return left + right;
}

ordinary.extra = "ordinary-property";

function describe(prefix, suffix) {
    return prefix + this.value + suffix;
}

function sumSeven(a, b, c, d, e, f, g) {
    return a + b + c + d + e + f + g;
}

async function doubleAsync(value) {
    return value * 2;
}

async function rejectAsync() {
    throw new Error("async hosted boom");
}

function* sequence(start) {
    yield start;
    yield start + 1;
    return start + 2;
}

function thrower() {
    throw new Error("hosted callable boom");
}

function Person(name) {
    this.name = name;
}

function NewTargetProbe() {
    this.targetName = new.target.name;
}

const arrow = value => value + 1;
const nested = { ordinary };

function echo(value) {
    return value;
}

function same(left, right) {
    return left === right;
}

function inspectCallback(callback) {
    callback.customProperty = "set-by-js";

    let constructable = true;
    try {
        new callback();
    } catch {
        constructable = false;
    }

    return {
        name: callback.name,
        length: callback.length,
        functionPrototype: Object.getPrototypeOf(callback) === Function.prototype,
        customProperty: callback.customProperty,
        constructable
    };
}

function invokeCallback(callback, left, right) {
    return callback(left, right);
}

function invokeCallbackVariadic(callback) {
    return callback("first", 2, true);
}

async function awaitCallback(callback, first, second) {
    return await callback(first, second);
}

function invokeCallbackWithReceiver(callback, value, prefix, suffix) {
    return callback.call({ value }, prefix, suffix);
}

function constructCallback(callback, value) {
    return new callback(value);
}

function readValue(value) {
    return value.value;
}

module.exports = {
    ordinary,
    describe,
    sumSeven,
    doubleAsync,
    rejectAsync,
    sequence,
    thrower,
    Person,
    NewTargetProbe,
    arrow,
    nested,
    echo,
    same,
    inspectCallback,
    invokeCallback,
    invokeCallbackVariadic,
    awaitCallback,
    invokeCallbackWithReceiver,
    constructCallback,
    readValue
};
