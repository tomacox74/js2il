const __nodeAssert = require('node:assert');

function assert(condition, message) {
    __nodeAssert.ok(condition, message);
}

assert.sameValue = function(actual, expected, message) {
    __nodeAssert.strictEqual(actual, expected, message);
};

assert.notSameValue = function(actual, unexpected, message) {
    __nodeAssert.notStrictEqual(actual, unexpected, message);
};

assert.strictEqual = assert.sameValue;
assert.notStrictEqual = assert.notSameValue;

assert.throws = function(expectedErrorConstructor, fn, message) {
    __nodeAssert.throws(fn, expectedErrorConstructor, message);
};

function compareArray(actual, expected) {
    if (!actual || !expected || actual.length !== expected.length) { return false; }
    for (var i = 0; i < actual.length; i++) {
        if (!Object.is(actual[i], expected[i])) { return false; }
    }
    return true;
}

assert.compareArray = function(actual, expected, message) {
    __nodeAssert.ok(compareArray(actual, expected), message || 'Expected arrays to match');
};

globalThis.assert = assert;
globalThis.compareArray = compareArray;
