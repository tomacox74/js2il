// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    "throw Expression" returns (throw, GetValue(Result(1)), empty), where 1
    evaluates Expression
es5id: 12.13_A2_T5
description: Throwing number
---*/

// CHECK#1
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

try{
  throw 13;
}
catch(e){
  console.log(!(e!==13));
}

// CHECK#2
var b=13;
try{
  throw b;
}
catch(e){
  console.log(!(e!==13));
}

// CHECK#3
try{
  throw 2.13;
}
catch(e){
  console.log(!(e!==2.13));
}

// CHECK#4
try{
  throw NaN;
}
catch(e){
  assert.sameValue(e, NaN, "e is NaN");
}

// CHECK#5
try{
  throw +Infinity;
}
catch(e){
  console.log(!(e!==+Infinity));
}

// CHECK#6
try{
  throw -Infinity;
}
catch(e){
  console.log(!(e!==-Infinity));
}

// CHECK#7
try{
  throw +0;
}
catch(e){
  console.log(!(e!==+0));
}

// CHECK#8
try{
  throw -0;
}
catch(e){
  assert.sameValue(e, -0);
}
