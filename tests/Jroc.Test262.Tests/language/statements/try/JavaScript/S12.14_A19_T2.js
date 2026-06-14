// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Catching system exceptions of different types with try statement
es5id: 12.14_A19_T2
description: Testing try/catch/finally syntax construction
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

  for (var i = 0; i < actual.length; i++) {
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

var fin=0;
// CHECK#1
try{
  throw (Error("hello"));
}
catch(e){
  if (e.toString()!=="Error: hello") throw new Test262Error('#1.1: Exception.toString()==="Error: hello". Actual: Exception is '+e);
}
finally{
  fin=1;
}
if (fin!==1) throw new Test262Error('#1.2: "finally" block must be evaluated'); 

// CHECK#2
fin=0;
try{
  throw (new Error("hello"));
}
catch(e){
  if (e.toString()!=="Error: hello") throw new Test262Error('#2.1: Exception.toString()==="Error: hello". Actual: Exception is '+e);
}
finally{
  fin=1;
}
if (fin!==1) throw new Test262Error('#2.2: "finally" block must be evaluated'); 

// CHECK#3
fin=0;
var c3=0;
try{
  throw EvalError(1);
}
catch(e){
  if (e.toString()!=="EvalError: 1") throw new Test262Error('#3.1: Exception.toString()==="EvalError: 1". Actual: Exception is '+e);
}
finally{
  fin=1;
}
if (fin!==1) throw new Test262Error('#3.2: "finally" block must be evaluated'); 

// CHECK#4
fin=0;
try{
  throw RangeError(1);
}
catch(e){
  if (e.toString()!=="RangeError: 1") throw new Test262Error('#4.1: Exception.toString()==="RangeError: 1". Actual: Exception is '+e);
}
finally{
  fin=1;
}
if (fin!==1) throw new Test262Error('#4.2: "finally" block must be evaluated'); 

// CHECK#5
fin=0;
try{
  throw ReferenceError(1);
}
catch(e){
  if (e.toString()!=="ReferenceError: 1") throw new Test262Error('#5.1: Exception.toString()==="ReferenceError: 1". Actual: Exception is '+e);
}
finally{
  fin=1;
}
if (fin!==1) throw new Test262Error('#5.2: "finally" block must be evaluated'); 

// CHECK#6
fin=0;
try{
  throw TypeError(1);
}
catch(e){
  if (e.toString()!=="TypeError: 1") throw new Test262Error('#6.1: Exception.toString()==="TypeError: 1". Actual: Exception is '+e);
}
finally{
  fin=1;
}
if (fin!==1) throw new Test262Error('#6.2: "finally" block must be evaluated'); 

// CHECK#7
fin=0;
try{
  throw URIError("message", "fileName", "1"); 
}
catch(e){
  if (e.toString()!=="URIError: message") throw new Test262Error('#7.1: Exception.toString()==="URIError: message". Actual: Exception is '+e);
}
finally{
  fin=1;
}
if (fin!==1) throw new Test262Error('#7.2: "finally" block must be evaluated');


console.log("pass");
