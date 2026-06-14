// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Using "try" with "catch" or "finally" statement within/without a "while"
    statement
es5id: 12.14_A10_T1
description: >
    Throwing exception while executing iteration statement placed into
    try Block
---*/


function assert(value, message) {
  console.log(!!value);
}
assert.sameValue = function(actual, expected, message) {
  console.log(Object.is(actual, expected));
};
assert.notSameValue = function(actual, unexpected, message) {
  console.log(!Object.is(actual, unexpected));
};
assert.compareArray = function(actual, expected, message) {
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
assert.throws = function(expectedErrorConstructor, func, message) {
  try {
    func();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedErrorConstructor);
  }
};

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

try {
  // CHECK#1
  var i=0;
  try{
  while(i<10){
    if(i===5) throw i;
    i++;
  }
  }
  catch(e){
    if(e!==5)throw new Test262Error('#1: Exception === 5. Actual:  Exception ==='+ e  );
  }

  console.log(true);
} catch (error) {
  console.log(false);
}
