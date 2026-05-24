// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: "\"try\" with \"catch\" or \"finally\" statement within/without an \"if\" statement"
es5id: 12.14_A8
description: Throwing exception within an "if" statement
---*/


function assert(value, message) {
  if (!value) {
    throw new Test262Error(message || 'Assertion failed');
  }
}

assert.sameValue = function(actual, expected, message) {
  if (!Object.is(actual, expected)) {
    throw new Test262Error(message || ('Expected SameValue but got ' + actual + ' and ' + expected));
  }
};

assert.notSameValue = function(actual, unexpected, message) {
  if (Object.is(actual, unexpected)) {
    throw new Test262Error(message || ('Expected different value but got ' + actual));
  }
};

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

// CHECK#1
var c1=1;
try{
  if(c1===1){
    throw "ex1";
    throw new Test262Error('#1.1: throw "ex1" lead to throwing exception');
  }
  throw new Test262Error('#1.2: throw "ex1" inside the "if" statement lead to throwing exception');
}
catch(er1){	
  if (er1!=="ex1") throw new Test262Error('#1.3: Exception ==="ex1". Actual:  Exception ==='+er1);
}

// CHECK#2
var c2=1;
if(c2===1){
  try{
    throw "ex1";
    throw new Test262Error('#2.1: throw "ex1" lead to throwing exception');
  }
  catch(er1){
    if(er1!="ex1") throw new Test262Error('#2.2: Exception ==="ex1". Actual:  Exception ==='+er1);
  }
}

console.log(true);
