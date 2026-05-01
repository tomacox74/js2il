// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Correct interpretation of DIGITS
es6id: 11.6
description: Identifier is $+ANY_DIGIT
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

var $0 = 0;
assert.sameValue($0, 0);

var $1 = 1;
assert.sameValue($1, 1);

var $2 = 2;
assert.sameValue($2, 2);

var $3 = 3;
assert.sameValue($3, 3);

var $4 = 4;
assert.sameValue($4, 4);

var $5 = 5;
assert.sameValue($5, 5);

var $6 = 6;
assert.sameValue($6, 6);

var $7 = 7;
assert.sameValue($7, 7);

var $8 = 8;
assert.sameValue($8, 8);

var $9 = 9;
assert.sameValue($9, 9);
