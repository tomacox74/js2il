// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    While evaluating "for ( ;  ; Expression) Statement", Statement is
    evaluated first and then Expression is evaluated
es5id: 12.6.3_A6
description: Using "(function(){throw "SecondExpression";})()" as an Expression
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

//////////////////////////////////////////////////////////////////////////////
//CHECK#1
try {
	for(;;(function(){throw "SecondExpression";})()){
        var __in__for = "reached";
    }
    throw new Test262Error('#1: (function(){throw "SecondExpression"}() lead to throwing exception');
} catch (e) {
	if (e !== "SecondExpression") {
		throw new Test262Error('#1: When for ( ;  ; Expression) Statement is evaluated Statement evaluates first then Expression evaluates');
	}
}
//
//////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////
//CHECK#2
if (__in__for !== "reached") {
	throw new Test262Error('#2: __in__for === "reached". Actual:  __in__for ==='+ __in__for  );
}
//
//////////////////////////////////////////////////////////////////////////////


console.log("pass");
