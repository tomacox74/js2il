// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    it isn't clear what specific requirements of the specificaiton are being tested here. This test should 
    probably be replaced by some more targeted tests.  AllenWB
es5id: 11.1.5-0-2
description: Object literal - multiple get set properties
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

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

  var s1 = "First getter";
  var s2 = "First setter";
  var s3 = "Second getter";
  var o;
  eval("o = {get foo(){ return s1;},set foo(arg){return s2 = s3}, get bar(){ return s3}, set bar(arg){ s3 = arg;}};");

assert.sameValue(o.foo, s1, 'o.foo');

  o.foo = 10;

assert.sameValue(s2, s3, 's2');
assert.sameValue(o.bar, s3, 'o.bar');

  o.bar = "Second setter";

assert.sameValue(o.bar, "Second setter", 'o.bar');
