// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es5id: 15.2.3.7-2-18
description: >
    Object.defineProperties - argument 'Properties' is the global
    object
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

var global = this;
var obj = {};
var result = false;

try {
  Object.defineProperty(this, "prop", {
    get: function() {
      result = (this === global);
      return {};
    },
    enumerable: true,
    configurable: true
  });

  Object.defineProperties(obj, this);
} catch (e) {
  if (!(e instanceof TypeError)) {
    throw new Test262Error("Expected TypeError, got " + e);
  }
  result = true;
} finally {
  delete this.prop;
}

assert(result, 'result !== true');
