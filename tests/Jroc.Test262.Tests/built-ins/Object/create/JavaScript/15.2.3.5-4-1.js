// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    create sets the [[Prototype]] of the created object to first parameter.
    This can be checked using isPrototypeOf, or getPrototypeOf.
es5id: 15.2.3.5-4-1
description: >
    Object.create sets the prototype of the passed-in object and adds
    new properties
---*/

// test262 execution-port helpers
var __test262_Object_is = Object.is;
var __test262_Object_getOwnPropertyDescriptor = Object.getOwnPropertyDescriptor;
var __test262_Object_defineProperty = Object.defineProperty;
var __test262_Object_keys = Object.keys;
var __test262_Object_hasOwnProperty = Object.prototype.hasOwnProperty;
var __test262_Array_isArray = Array.isArray;

function __test262_compareArray(actual, expected) {
  if (!__test262_Array_isArray(actual) || !__test262_Array_isArray(expected) || actual.length !== expected.length) {
    return false;
  }
  for (var i = 0; i < actual.length; i++) {
    if (!__test262_Object_is(actual[i], expected[i])) {
      return false;
    }
  }
  return true;
}

var assert = function assert(condition) {
  console.log(Boolean(condition));
};

function base() {}
var b = new base();
var prop = new Object();
var d = Object.create(b, {
  "x": {
    value: true,
    writable: false
  },
  "y": {
    value: "str",
    writable: false
  }
});

assert.sameValue(Object.getPrototypeOf(d), b, 'Object.getPrototypeOf(d)');
assert.sameValue(b.isPrototypeOf(d), true, 'b.isPrototypeOf(d)');
assert.sameValue(d.x, true, 'd.x');
assert.sameValue(d.y, "str", 'd.y');
assert.sameValue(b.x, undefined, 'b.x');
assert.sameValue(b.y, undefined, 'b.y');
