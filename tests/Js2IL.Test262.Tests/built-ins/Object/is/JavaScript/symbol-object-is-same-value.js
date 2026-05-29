// Copyright (C) 2013 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-object.is
description: >
    Object.is/SameValue: Symbol
features: [Object.is, Symbol]
---*/

// test262 execution-port helpers
function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

var assert = function assert(condition) {
  console.log(Boolean(condition));
};

assert.sameValue = function(actual, expected) {
  console.log(Object.is(actual, expected));
};

assert.notSameValue = function(actual, unexpected) {
  console.log(!Object.is(actual, unexpected));
};

assert.throws = function(ExpectedError, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof ExpectedError || error.constructor === ExpectedError || error.name === ExpectedError.name);
  }
};

var symA = Symbol('66');
var symB = Symbol('66');


assert.sameValue(Object.is(symA, symA), true, "`Object.is(symA, symA)` returns `true`");
assert.sameValue(Object.is(symB, symB), true, "`Object.is(symB, symB)` returns `true`");
assert.sameValue(Object.is(symA, symB), false, "`Object.is(symA, symB)` returns `false`");
