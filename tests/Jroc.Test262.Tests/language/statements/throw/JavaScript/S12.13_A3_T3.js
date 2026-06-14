// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: 1. Evaluate Expression
es5id: 12.13_A3_T3
description: Evaluating number expression
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
  throw 10+3;
}
catch(e){
  console.log(!(e!==13));
}

// CHECK#2
var b=10;
var a=3;
try{
  throw a+b;
}
catch(e){
  console.log(!(e!==13));
}

// CHECK#3
try{
  throw 3.15-1.02;
}
catch(e){
  console.log(!(e!==2.13));
}

// CHECK#4
try{
  throw 2*2;
}
catch(e){
  console.log(!(e!==4));
}

// CHECK#5
try{
  throw 1+Infinity;
}
catch(e){
  console.log(!(e!==+Infinity));
}

// CHECK#6
try{
  throw 1-Infinity;
}
catch(e){
  console.log(!(e!==-Infinity));
}

// CHECK#7
try{
  throw 10/5;
}
catch(e){
  console.log(!(e!==2));
}

// CHECK#8
try{
  throw 8>>2;
}
catch(e){
  console.log(!(e!==2));
}

// CHECK#9
try{
  throw 2<<2;
}
catch(e){
  console.log(!(e!==8));
}

// CHECK#10
try{
  throw 123%100;
}
catch(e){
  console.log(!(e!==23));
}
