// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es5id: 15.2.3.7-5-b-11
description: >
    Object.defineProperties - 'enumerable' property of 'descObj' is
    own data property that overrides an inherited accessor property
    (8.10.5 step 3.a)
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
var proto = {};
var accessed = false;

Object.defineProperty(proto, "enumerable", {
  get: function() {
    return true;
  }
});

var Con = function() {};
Con.prototype = proto;
var descObj = new Con();

Object.defineProperty(descObj, "enumerable", {
  value: false
});

Object.defineProperties(obj, {
  prop: descObj
});

for (var property in obj) {
  if (property === "prop") {
    accessed = true;
  }
}

assert.sameValue(accessed, false, 'accessed');
