// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The production TryStatement : try Block Catch is evaluated as follows: 2.
    If Result(1).type is not throw, return Result(1)
es5id: 12.14_A1
description: >
    Executing TryStatement : try Block Catch. The statements doesn't
    cause actual exceptions
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
try {
  var x=0;
}
catch (e) {
  throw new Test262Error('#1: If Result(1).type is not throw, return Result(1). Actual: 4 Return(Result(3))');
}

// CHECK#2
var c1=0;
try{
  var x1=1;
}
finally
{
  c1=1;
}
if(x1!==1){
  throw new Test262Error('#2.1: "try" block must be evaluated. Actual: try Block has not been evaluated');
}
if (c1!==1){
  throw new Test262Error('#2.2: "finally" block must be evaluated. Actual: finally Block has not been evaluated');
}

// CHECK#3
var c2=0;
try{
  var x2=1;
}
catch(e){
  throw new Test262Error('#3.1: If Result(1).type is not throw, return Result(1). Actual: 4 Return(Result(3))');	
}
finally{
  c2=1;
}
if(x2!==1){
  throw new Test262Error('#3.2: "try" block must be evaluated. Actual: try Block has not been evaluated');
}
if (c2!==1){
  throw new Test262Error('#3.3: "finally" block must be evaluated. Actual: finally Block has not been evaluated');
}

console.log(true);
