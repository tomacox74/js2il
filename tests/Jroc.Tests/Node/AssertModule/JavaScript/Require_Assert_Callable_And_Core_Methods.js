"use strict";

const assert = require("node:assert");
const legacyAssert = require("assert");

assert(true);
assert.ok(1);
assert.equal("1", 1);
assert.strictEqual(NaN, NaN);
assert.notStrictEqual(0, -0);
assert.match("jroc", /^jr/);
assert.doesNotMatch("jroc", /node/);
assert.throws(() => {
    throw new Error("expected");
}, /expected/);
assert.doesNotThrow(() => 42);

console.log("callable:", typeof assert);
console.log("same module:", assert === legacyAssert);
console.log("ok identity:", assert.ok === assert);
console.log("strict callable:", typeof assert.strict);
