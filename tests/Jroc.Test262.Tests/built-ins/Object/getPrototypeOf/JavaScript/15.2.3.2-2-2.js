// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Let 'x' be the return value from getPrototypeOf when called on d.
    Then, x.isPrototypeOf(d) must be true.
es5id: 15.2.3.2-2-2
description: >
    Object.getPrototypeOf returns the [[Prototype]] of its parameter
    (custom object)
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

function derived() {}
derived.prototype = new base();

var d = new derived();
var x = Object.getPrototypeOf(d);

assert.sameValue(x.isPrototypeOf(d), true, 'x.isPrototypeOf(d)');
