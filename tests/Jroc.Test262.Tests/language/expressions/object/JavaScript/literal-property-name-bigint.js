// Copyright (C) 2020 Igalia S.L, Toru Nagashima. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

// Property

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
let o = { 999999999999999999n: true }; // greater than max safe integer

assert.sameValue(o["999999999999999999"], true,
    "the property name must be the string representation of the numeric value.");

// MethodDeclaration

o = { 1n() { return "bar"; } };
assert.sameValue(o["1"](), "bar",
    "the property name must be the string representation of the numeric value.");

class C {
  1n() { return "baz"; }
}

let c = new C();
assert.sameValue(c["1"](), "baz",
    "the property name must be the string representation of the numeric value.");

// Destructuring

let { 1n: a } = { "1": "foo" };
assert.sameValue(a, "foo",
    "the property name must be the string representation of the numeric value.");
