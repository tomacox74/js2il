// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    While evaluating "for (ExpressionNoIn; FirstExpression; SecondExpression)
    Statement", ExpressionNoIn is evaulated first, FirstExpressoin is
    evaluated second
es5id: 12.6.3_A3
description: Using "(function(){throw "FirstExpression"})()" as FirstExpression
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

var __in__NotInExpression__, __in__NotInExpression__2, __in__for;

//////////////////////////////////////////////////////////////////////////////
//CHECK#1
try {
	for((function(){__in__NotInExpression__ = "checked";__in__NotInExpression__2 = "passed";})(); (function(){throw "FirstExpression"})(); (function(){throw "SecondExpression"})()) {
		__in__for="reached";
	}
	throw new Test262Error('#1: (function(){throw "SecondExpression"} lead to throwing exception');
} catch (e) {
	if (e !== "FirstExpression") {
		throw new Test262Error('#1: When for (ExpressionNoIn ; FirstExpression ; SecondExpression) Statement is evaluated first evaluates ExpressionNoIn then FirstExpression');
	}
}
//
//////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////
//CHECK#2
if ((__in__NotInExpression__ !== "checked")&(__in__NotInExpression__2!=="passed")) {
	throw new Test262Error('#2: (__in__NotInExpression__ === "checked")&(__in__NotInExpression__2==="passed")');
}
//
//////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////
//CHECK#3
if (typeof __in__for !== "undefined") {
	throw new Test262Error('#3: typeof __in__for === "undefined". Actual:  typeof __in__for ==='+ typeof __in__for  );
}
//
//////////////////////////////////////////////////////////////////////////////


console.log("pass");
