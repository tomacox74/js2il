// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    "break" within a "do-while" Statement is allowed and performed as
    described in 12.8
es5id: 12.6.1_A4_T2
description: "\"break\" and VariableDeclaration within a \"do-while\" statement"
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

do_out : do {
    var __in__do__before__break="black";
    do_in : do {
        var __in__do__IN__before__break="hole";
        break do_in; 
        var __in__do__IN__after__break="sun";
    } while (0);
    var __in__do__after__break="won't you come";
} while(2==1);

//////////////////////////////////////////////////////////////////////////////
//CHECK#1
console.log(!(!(__in__do__before__break&&__in__do__IN__before__break&&!__in__do__IN__after__break&&__in__do__after__break)));
//
//////////////////////////////////////////////////////////////////////////////
