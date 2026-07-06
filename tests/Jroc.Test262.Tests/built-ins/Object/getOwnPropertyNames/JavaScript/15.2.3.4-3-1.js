// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es5id: 15.2.3.4-3-1
description: >
    Object.getOwnPropertyNames - elements of the returned array start
    from index 0
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

var obj = {
  prop1: 1001
};

var arr = Object.getOwnPropertyNames(obj);

assert(arr.hasOwnProperty(0), 'arr.hasOwnProperty(0) !== true');
assert.sameValue(arr[0], "prop1", 'arr[0]');
