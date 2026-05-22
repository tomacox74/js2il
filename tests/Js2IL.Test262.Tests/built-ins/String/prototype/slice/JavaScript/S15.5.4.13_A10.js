// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: The String.prototype.slice.length property has the attribute ReadOnly
es5id: 15.5.4.13_A10
description: >
    Checking if varying the String.prototype.slice.length property
    fails
includes: [propertyHelper.js]
---*/

function assert(value) {
  console.log(!!value);
}

assert.sameValue = function(actual, expected) {
  console.log(Object.is(actual, expected));
};

assert.notSameValue = function(actual, unexpected) {
  console.log(!Object.is(actual, unexpected));
};

function compareArray(actual, expected) {
  if (!Array.isArray(actual) || !Array.isArray(expected) || actual.length !== expected.length) {
    return false;
  }

  for (let i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      return false;
    }
  }

  return true;
}

assert.compareArray = function(actual, expected) {
  console.log(compareArray(actual, expected));
};

assert.throws = function(expectedCtor, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedCtor);
  }
};

function verifyProperty(obj, name, desc) {
  var actual = Object.getOwnPropertyDescriptor(obj, name);
  if (actual === undefined) {
    console.log(false);
    return;
  }

  var ok = true;

  if (Object.prototype.hasOwnProperty.call(desc, 'value')) {
    ok = ok && Object.is(actual.value, desc.value);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'writable')) {
    ok = ok && Object.is(actual.writable, desc.writable);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'enumerable')) {
    ok = ok && Object.is(actual.enumerable, desc.enumerable);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'configurable')) {
    ok = ok && Object.is(actual.configurable, desc.configurable);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'get')) {
    ok = ok && Object.is(actual.get, desc.get);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'set')) {
    ok = ok && Object.is(actual.set, desc.set);
  }

  console.log(ok);
}

function verifyNotWritable(obj, name, verifyProp, value) {
  var oldValue = obj[name];
  var newValue = value || "unlikelyValue";
  try {
    obj[name] = newValue;
  } catch (error) {}
  if (!Object.is(obj[name], oldValue)) {
    throw new Test262Error("Expected " + name + " to be non-writable");
  }
  if (verifyProp) {
    verifyProperty(obj, name, { writable: false });
  }
}

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

//////////////////////////////////////////////////////////////////////////////
//CHECK#1
if (!(String.prototype.slice.hasOwnProperty('length'))) {
  throw new Test262Error('#1: String.prototype.slice.hasOwnProperty(\'length\') return true. Actual: ' + String.prototype.slice.hasOwnProperty('length'));
}
//
//////////////////////////////////////////////////////////////////////////////

var __obj = String.prototype.slice.length;

verifyNotWritable(String.prototype.slice, "length", null, function() {
  return "shifted";
});

//////////////////////////////////////////////////////////////////////////////
//CHECK#2
if (String.prototype.slice.length !== __obj) {
  throw new Test262Error('#2: __obj = String.prototype.slice.length; String.prototype.slice.length = function(){return "shifted";}; String.prototype.slice.length === __obj. Actual: ' + String.prototype.slice.length);
}
//
//////////////////////////////////////////////////////////////////////////////
console.log(true);
