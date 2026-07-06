// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The abtract operation ToPropertyDescriptor is used to package the
    into a property desc. Step 10 of ToPropertyDescriptor throws a TypeError
    if the property desc ends up having a mix of accessor and data property elements.
es5id: 15.2.3.6-3-1
description: >
    Object.defineProperty throws TypeError if desc has 'get' and
    'value' present(8.10.5 step 9.a)
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

var o = {};

// dummy getter
var getter = function() {
  return 1;
}
var desc = {
  get: getter,
  value: 101
};
assert.throws(TypeError, function() {
  Object.defineProperty(o, "foo", desc);
});
assert.sameValue(o.hasOwnProperty("foo"), false, 'o.hasOwnProperty("foo")');
