// Copyright (C) 2015 Jordan Harband. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-object.entries
description: >
    Object.entries should not have its behavior impacted by modifications to Object.keys
author: Jordan Harband
---*/
// test262 execution-port helpers
var __test262_Object_is = Object.is;
var __test262_Array_isArray = Array.isArray;
var __test262_Object_keys = Object.keys;
var __test262_getOwnPropertyDescriptor = Object.getOwnPropertyDescriptor;
var __test262_defineProperty = Object.defineProperty;
var __test262_hasOwnProperty = Object.prototype.hasOwnProperty;

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

function __test262_deepEqual(actual, expected) {
  if (__test262_Object_is(actual, expected)) {
    return true;
  }
  if (__test262_Array_isArray(actual) && __test262_Array_isArray(expected)) {
    if (actual.length !== expected.length) {
      return false;
    }
    for (var i = 0; i < actual.length; i++) {
      if (!__test262_deepEqual(actual[i], expected[i])) {
        return false;
      }
    }
    return true;
  }
  if (actual && expected && typeof actual === "object" && typeof expected === "object") {
    var actualKeys = __test262_Object_keys(actual);
    var expectedKeys = __test262_Object_keys(expected);
    if (!__test262_compareArray(actualKeys, expectedKeys)) {
      return false;
    }
    for (var j = 0; j < actualKeys.length; j++) {
      var key = actualKeys[j];
      if (!__test262_deepEqual(actual[key], expected[key])) {
        return false;
      }
    }
    return true;
  }
  return false;
}

function __test262_isWritable(obj, name, verifyProp, value) {
  var newValue = arguments.length > 3 ? value : "unlikelyValue";
  var verifiedName = verifyProp || name;
  var oldValue = obj[verifiedName];
  var writeSucceeded = false;
  try {
    obj[name] = newValue;
    writeSucceeded = __test262_Object_is(obj[verifiedName], newValue);
  } catch (error) {
    writeSucceeded = false;
  }
  try {
    obj[name] = oldValue;
  } catch (error) {
  }
  return writeSucceeded;
}

function __test262_isEnumerable(obj, name) {
  for (var key in obj) {
    if (key === name) {
      return true;
    }
  }
  return false;
}

function __test262_isConfigurable(obj, name) {
  var descriptor = __test262_getOwnPropertyDescriptor(obj, name);
  if (!descriptor) {
    return false;
  }
  try {
    delete obj[name];
    var deleted = !__test262_hasOwnProperty.call(obj, name);
    __test262_defineProperty(obj, name, descriptor);
    return deleted;
  } catch (error) {
    return false;
  }
}

var assert = function assert(condition) {
  console.log(Boolean(condition));
};

assert.deepEqual = function (actual, expected) {
  console.log(__test262_deepEqual(actual, expected));
};

function deepEqual(actual, expected) {
  return __test262_deepEqual(actual, expected);
}

function verifyEqualTo(obj, name, value) {
  var actual = __test262_getOwnPropertyDescriptor(obj, name);
  var passed = !!actual && __test262_Object_is(actual.value, value);
  console.log(passed);
  return passed;
}

function reportCompare() {
}

Test262Error.prototype.name = "Test262Error";

function print() {
  console.log.apply(console, arguments);
}

function fakeObjectKeys() {
  throw new Test262Error('The overriden version of Object.keys was called!');
}

Object.keys = fakeObjectKeys;

assert.sameValue(Object.keys, fakeObjectKeys, 'Sanity check failed: could not modify the global Object.keys');
assert.sameValue(Object.entries({
  a: 1
}).length, 1, 'Expected object with 1 key to have 1 entry');
