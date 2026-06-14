// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    "break" within a "while" Statement is allowed and performed as described
    in 12.8
es5id: 12.6.2_A4_T3
description: "\"break\" and VariableDeclaration within a \"while\" Statement"
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

do_out : while(1===1) {
    if (__in__do__before__break) break;
    var __in__do__before__break="once";
    do_in : while (1) {
        var __in__do__IN__before__break="in";
        break do_out;
        var __in__do__IN__after__break="the";
    } ;
    var __in__do__after__break="lifetime";
} ;

//////////////////////////////////////////////////////////////////////////////
//CHECK#1
if (!(__in__do__before__break&&__in__do__IN__before__break&&!__in__do__IN__after__break&&!__in__do__after__break)) {
	throw new Test262Error('#1: Break inside do-while is allowed as its described at standard');
}
//
//////////////////////////////////////////////////////////////////////////////

console.log(true);
