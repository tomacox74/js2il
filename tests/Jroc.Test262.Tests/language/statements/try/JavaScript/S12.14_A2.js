// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Throwing exception with "throw" and catching it with "try" statement
es5id: 12.14_A2
description: >
    Checking if execution of "catch" catches an exception thrown with
    "throw"
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
  throw "catchme";	
  throw new Test262Error('#1: throw "catchme" lead to throwing exception');
}
catch(e){}

// CHECK#2
var c2=0;
try{
  try{
    throw "exc";
    throw new Test262Error('#2.1: throw "exc" lead to throwing exception');
  }finally{
    c2=1;
  }
}
catch(e){
  if (c2!==1){
    throw new Test262Error('#2.2: "finally" block must be evaluated');
  }
}
 
// CHECK#3
var c3=0;
try{
  throw "exc";
  throw new Test262Error('#3.1: throw "exc" lead to throwing exception');
}
catch(err){  	
  var x3=1;
}
finally{
  c3=1;
}
if (x3!==1){
  throw new Test262Error('#3.2: "catch" block must be evaluated');
}  
if (c3!==1){
  throw new Test262Error('#3.3: "finally" block must be evaluated');
}

console.log(true);
