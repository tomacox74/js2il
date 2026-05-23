// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Expression from "while" IterationStatement is evaluated first; "false",
    "0", "null", "undefined" and "empty" strings used as the Expression are
    evaluated to "false"
es5id: 12.6.2_A1
description: Evaluating various Expressions
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

var __in__do;

while ( false ) __in__do=1;

//////////////////////////////////////////////////////////////////////////////
//CHECK#1
console.log(!(__in__do !== undefined));
//
//////////////////////////////////////////////////////////////////////////////

while ( 0 ) __in__do=2;

//////////////////////////////////////////////////////////////////////////////
//CHECK#2
console.log(!(__in__do !== undefined));
//
//////////////////////////////////////////////////////////////////////////////

while ( "" ) __in__do=3;

//////////////////////////////////////////////////////////////////////////////
//CHECK#3
console.log(!(__in__do !== undefined));
//
//////////////////////////////////////////////////////////////////////////////

while ( null ) __in__do=4;

//////////////////////////////////////////////////////////////////////////////
//CHECK#4
console.log(!(__in__do !== undefined));
//
//////////////////////////////////////////////////////////////////////////////

while ( undefined ) __in__do=35;

//////////////////////////////////////////////////////////////////////////////
//CHECK#5
console.log(!(__in__do !== undefined));
//
//////////////////////////////////////////////////////////////////////////////
