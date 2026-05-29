// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    While evaluating "for (ExpressionNoIn; Expression; Expression)
    Statement", ExpressionNoIn is evaulated first
es5id: 12.6.3_A2
description: Using "(function(){throw "NoInExpression"})()" as ExpressionNoIn
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
	for((function(){throw "NoInExpression";})(); (function(){throw "FirstExpression";})(); (function(){throw "SecondExpression";})()) {
		var in_for = "reached";
	}
	throw new Test262Error('#1: (function(){throw "NoInExpression";})() lead to throwing exception');
} catch (e) {
	if (e !== "NoInExpression") {
		throw new Test262Error('#1: When for (ExpressionNoIn ; Expression ; Expression) Statement is evaluated ExpressionNoIn evaluates first');
	}
}
//
//////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////
//CHECK#2
if (in_for !== undefined) {
	throw new Test262Error('#2: in_for === undefined. Actual:  in_for ==='+ in_for  );
}
//
//////////////////////////////////////////////////////////////////////////////


console.log("pass");
