// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Step 4 of defineProperty calls the [[DefineOwnProperty]] internal method
    of O to define the property. For non-configurable properties, step 9a of
    [[DefineOwnProperty]] rejects changing the kind of a property.
es5id: 15.2.3.6-4-12
description: >
    Object.defineProperty throws TypeError when changing
    non-configurable data properties to accessor properties
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

function verifyProperty(obj, name, desc) {
  var actual = __test262_Object_getOwnPropertyDescriptor(obj, name);
  var ok = !!actual;
  if ('value' in desc) ok = ok && __test262_Object_is(actual.value, desc.value);
  if ('writable' in desc) ok = ok && actual.writable === desc.writable;
  if ('enumerable' in desc) ok = ok && actual.enumerable === desc.enumerable;
  if ('configurable' in desc) ok = ok && actual.configurable === desc.configurable;
  if ('get' in desc) ok = ok && actual.get === desc.get;
  if ('set' in desc) ok = ok && actual.set === desc.set;
  console.log(ok);
  return ok;
}

var assert = function assert(condition) {
  console.log(Boolean(condition));
};

assert.sameValue = function(actual, expected) {
  console.log(__test262_Object_is(actual, expected));
};

assert.notSameValue = function(actual, unexpected) {
  console.log(!__test262_Object_is(actual, unexpected));
};

assert.compareArray = function(actual, expected) {
  console.log(__test262_compareArray(actual, expected));
};

assert.throws = function(ExpectedError, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof ExpectedError || error.constructor === ExpectedError || error.name === ExpectedError.name);
  }
};
var o = {};

// create a data valued property; all other attributes default to false.
var d1 = {
  value: 101,
  configurable: false
};
Object.defineProperty(o, "foo", d1);

// changing "foo" to be an accessor should fail, since [[Configurable]]
// on the original property will be false.

// dummy getter
var getter = function() {
  return 1;
}

var desc = {
  get: getter
};
assert.throws(TypeError, function() {
  Object.defineProperty(o, "foo", desc);
});
// the property should remain a data valued property.
var d2 = Object.getOwnPropertyDescriptor(o, "foo");
assert.sameValue(d2.value, 101, 'd2.value');
assert.sameValue(d2.writable, false, 'd2.writable');
assert.sameValue(d2.enumerable, false, 'd2.enumerable');
assert.sameValue(d2.configurable, false, 'd2.configurable');
