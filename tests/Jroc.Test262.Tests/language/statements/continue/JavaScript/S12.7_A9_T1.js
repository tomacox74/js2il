// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Continue inside of try-catch nested in a loop is allowed
es5id: 12.7_A9_T1
description: >
    Using "continue Identifier" within catch Block that is within a
    loop
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

var x=0,y=0;

(function(){
FOR : for(;;){
	try{
		x++;
		if(x===10)return;
		throw 1;
	} catch(e){
		continue FOR;
	}	
}
})();

//////////////////////////////////////////////////////////////////////////////
//CHECK#1
if (x!==10) {
	throw new Test262Error('#1: Continue inside of try-catch nested in loop is allowed');
}
//
//////////////////////////////////////////////////////////////////////////////

console.log(true);
