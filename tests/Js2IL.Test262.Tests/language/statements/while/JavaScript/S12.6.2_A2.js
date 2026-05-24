// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    While evaluating The production IterationStatement: "while ( Expression )
    Statement", Expression is evaluated first
es5id: 12.6.2_A2
description: Evaluating Statement with error Expression
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

try {
	while ((function(){throw 1})()) __in__while = "reached"; 
	throw new Test262Error('#1: \'while ((function(){throw 1})()) __in__while = "reached"\' lead to throwing exception');
} catch (e) {
	if (e !== 1) {
		throw new Test262Error('#1: Exception === 1. Actual:  Exception ==='+e);
	}
}

//////////////////////////////////////////////////////////////////////////////
//CHECK#1
if (typeof __in__while !== "undefined") {
	throw new Test262Error('#1.1: typeof __in__while === "undefined". Actual: typeof __in__while ==='+typeof __in__while);
}
//
//////////////////////////////////////////////////////////////////////////////

console.log(true);
