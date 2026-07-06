// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 19.1.2.18
description: Object.setPrototypeOf invoked with a non-extensible object
info: |
    1. Let O be RequireObjectCoercible(O).
    2. ReturnIfAbrupt(O).
    3. If Type(proto) is neither Object nor Null, throw a TypeError exception.
    4. If Type(O) is not Object, return O.
    5. Let status be O.[[SetPrototypeOf]](proto).
    6. ReturnIfAbrupt(status).
    7. If status is false, throw a TypeError exception.
    8. Return O.
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

var propValue = {};
var newProto = {
  test262prop: propValue
};
var obj = {};
var result;

result = Object.setPrototypeOf(obj, newProto);

assert.sameValue(result, obj, 'Return value');
assert(
  !Object.prototype.hasOwnProperty.call(obj, 'test262prop'),
  "'test262prop' isn't copied to an own property"
);
assert.sameValue(obj.test262prop, propValue);
