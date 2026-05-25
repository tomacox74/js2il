// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    While evaluating "do Statement while ( Expression )", Statement is
    evaluated first and only after it is done Expression is checked
es5id: 12.6.1_A2
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

var __in__do;

try {
	do __in__do = "reached"; while (abbracadabra);
	throw new Test262Error('#1: \'do __in__do = "reached"; while (abbracadabra)\' lead to throwing exception');
} catch (e) {
    if (e instanceof Test262Error) throw e;
}

//////////////////////////////////////////////////////////////////////////////
//CHECK#1
if (__in__do !== "reached") {
	throw new Test262Error('#1.1: __in__do === "reached". Actual:  __in__do ==='+ __in__do  );
}
//
//////////////////////////////////////////////////////////////////////////////

console.log(true);
