// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Using "try" with "catch" or "finally" statement within/without a "for-in"
    statement
es5id: 12.14_A12_T1
description: Loop inside try Block, where throw exception
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
  var x;
  var mycars = new Array();
  mycars[0] = "Saab";
  mycars[1] = "Volvo";
  mycars[2] = "BMW";

  // CHECK#1
  try{
    for (x in mycars){
      if (mycars[x]==="BMW") throw "ex";
    }
  }
  catch(e){
    if(e!=="ex")throw new Test262Error('#1: Exception ==="ex". Actual:  Exception ==='+ e  );
  }

  console.log(true);
} catch (error) {
  console.log(false);
}
