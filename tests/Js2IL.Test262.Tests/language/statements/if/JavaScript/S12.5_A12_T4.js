// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Embedded "if/else" constructions are allowed
es5id: 12.5_A12_T4
description: Using embedded "if" into "if" constructions
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

//CHECK# 1
if(true)
  if (false)
    throw new Test262Error('#1.1: At embedded "if/else" constructions engine must select right branches');

//CHECK# 2
var c=0;
if(true)
  if (true)
    c=2;
if (c!==2)
    throw new Test262Error('#2: At embedded "if/else" constructions engine must select right branches');

//CHECK# 3
if(false)
  if (true)
    throw new Test262Error('#3.1: At embedded "if/else" constructions engine must select right branches');

//CHECK# 4
if(false)
  if (true)
    throw new Test262Error('#4.1: At embedded "if/else" constructions engine must select right branches');

console.log(true);
