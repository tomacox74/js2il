// Copyright (C) 2011 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-runtime-semantics-forin-div-ofbodyevaluation-lhs-stmt-iterator-lhskind-labelset
es6id: 13.7.5.13
description: >
    let ForDeclaration: creates a fresh binding per iteration
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

assert.compareArray = function(actual, expected) {
  if (!Array.isArray(actual) || !Array.isArray(expected) || actual.length !== expected.length) {
    console.log(false);
    return;
  }

  for (let i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      console.log(false);
      return;
    }
  }

  console.log(true);
};

assert.throws = function(expectedCtor, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedCtor);
  }
};

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

var fns = {};
var obj = Object.create(null);
obj.a = 1;
obj.b = 1;
obj.c = 1;

for (let x in obj) {
  // Store function objects as properties of an object so that their return
  // value may be verified regardless of the for-in statement's enumeration
  // order.
  fns[x] = function() { return x; };
}

assert.sameValue(typeof fns.a, 'function', 'property definition: "a"');
assert.sameValue(fns.a(), 'a');
assert.sameValue(typeof fns.b, 'function', 'property definition: "b"');
assert.sameValue(fns.b(), 'b');
assert.sameValue(typeof fns.c, 'function', 'property definition: "c"');
assert.sameValue(fns.c(), 'c');
