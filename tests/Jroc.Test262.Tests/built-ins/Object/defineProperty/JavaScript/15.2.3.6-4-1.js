// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Step 4 of defineProperty calls the [[DefineOwnProperty]] internal method
    of O passing 'true' for the Throw flag. In this case, step 3 of
    [[DefineOwnProperty]] requires that it throw a TypeError exception when
    current is undefined and extensible is false. The value of desc does not
    matter.
es5id: 15.2.3.6-4-1
description: >
    Object.defineProperty throws TypeError when adding properties to
    non-extensible objects(8.12.9 step 3)
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
Object.preventExtensions(o);
assert.throws(TypeError, function() {
  var desc = {
    value: 1
  };
  Object.defineProperty(o, "foo", desc);
});
assert.sameValue(o.hasOwnProperty("foo"), false, 'o.hasOwnProperty("foo")');
