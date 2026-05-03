// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The production StatementList  Statement is evaluated as follows
    1. Evaluate Statement.
    2. If an exception was thrown, return (throw, V, empty) where V is the exception
es5id: 12.1_A2
description: Throwing exception within a Block
---*/

//////////////////////////////////////////////////////////////////////////////
//CHECK#1
function assert(value, message) {
  console.log(!!value);
}
assert.sameValue = function(actual, expected, message) {
  console.log(Object.is(actual, expected));
};
assert.notSameValue = function(actual, unexpected, message) {
  console.log(!Object.is(actual, unexpected));
};
assert.throws = function(expectedErrorConstructor, func, message) {
  try {
    func();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedErrorConstructor);
  }
};

var Test262Error = function(message) {
  this.name = 'Test262Error';
  this.message = message || '';
};
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

try {
	{
		throw { name: "ReferenceError" };
	}
	console.log(false);
} catch (error) {
	console.log(error && error.name === "ReferenceError");
}

//
//////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////
//CHECK#2
try {
    throw "catchme";	
    throw new Test262Error('#2: throw "catchme" lead to throwing exception');
} catch (e) {
	if (e!=="catchme") {
		throw new Test262Error('#2.1: Exception === "catchme". Actual:  Exception ==='+ e  );
	}
}

//
//////////////////////////////////////////////////////////////////////////////

console.log(true);
