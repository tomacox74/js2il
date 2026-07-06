// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es5id: 15.2.3.7-5-b-101
description: >
    Object.defineProperties - value of 'configurable' property of
    'descObj' is Number object (8.10.5 step 4.b)
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

var obj = {};

Object.defineProperties(obj, {
  property: {
    configurable: new Number(-123)
  }
});
var preCheck = obj.hasOwnProperty("property");
delete obj.property;

assert(preCheck, 'preCheck !== true');
assert.sameValue(obj.hasOwnProperty("property"), false, 'obj.hasOwnProperty("property")');
