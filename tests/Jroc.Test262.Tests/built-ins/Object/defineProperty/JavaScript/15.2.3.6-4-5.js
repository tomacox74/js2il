// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Step 4 of defineProperty calls the [[DefineOwnProperty]] internal method
    of O to define the property. Step 6 of [[DefineOwnProperty]] returns if
    every field of desc also occurs in current and every field in desc has
    the same value as current.
es5id: 15.2.3.6-4-5
description: >
    Object.defineProperty is no-op if current and desc are the same
    data desc
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

function sameDataDescriptorValues(d1, d2) {
  return (d1.value === d2.value &&
    d1.enumerable === d2.enumerable &&
    d1.writable === d2.writable &&
    d1.configurable === d2.configurable);
}

var o = {};

// create a data valued property with the following attributes:
// value: 101, enumerable: true, writable: true, configurable: true
o["foo"] = 101;

// query for, and save, the desc. A subsequent call to defineProperty
// with the same desc should not disturb the property definition.
var d1 = Object.getOwnPropertyDescriptor(o, "foo");

// now, redefine the property with the same descriptor
// the property defintion should not get disturbed.
var desc = {
  value: 101,
  enumerable: true,
  writable: true,
  configurable: true
};
Object.defineProperty(o, "foo", desc);

var d2 = Object.getOwnPropertyDescriptor(o, "foo");

assert.sameValue(sameDataDescriptorValues(d1, d2), true, 'sameDataDescriptorValues(d1, d2)');
