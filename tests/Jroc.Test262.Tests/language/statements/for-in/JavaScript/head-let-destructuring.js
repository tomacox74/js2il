// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-iteration-statements
es6id: 13.7
description: >
  The token sequence `let [`is interpreted as the beginning of a destructuring
  binding pattern
info: |
  Syntax

  IterationStatement[Yield, Return]:

    for ( [lookahead ∉ { let [ } ] LeftHandSideExpression[?Yield] in
      Expression[+In, ?Yield] ) Statement[?Yield, ?Return]

    for ( ForDeclaration[?Yield] in Expression[+In, ?Yield] )
      Statement[?Yield, ?Return]
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

var obj = Object.create(null);
var value;

obj.key = 1;

for ( let[x] in obj ) {
  value = x;
}

assert.sameValue(typeof x, 'undefined', 'binding is block-scoped');
assert.sameValue(value, 'k');
