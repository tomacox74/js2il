// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    If (Evaluate Statement).type is "continue" and (Evaluate
    Statement).target is in the current label set, iteration of labeled
    "var-loop" breaks
es5id: 12.6.3_A11.1_T2
description: Embedded loops
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

var __str;
__str="";

outer : for(var index=0; index<4; index+=1) {
    nested : for(var index_n=0; index_n<=index; index_n++) {
	if (index*index_n == 6)continue nested;
	__str+=""+index+index_n;
    } 
}

//////////////////////////////////////////////////////////////////////////////
//CHECK#1
if (__str !== "001011202122303133") {
	throw new Test262Error('#1: __str === "001011202122303133". Actual:  __str ==='+ __str  );
}
//
//////////////////////////////////////////////////////////////////////////////

__str="";

outer : for(var index=0; index<4; index+=1) {
    nested : for(var index_n=0; index_n<=index; index_n++) {
	if (index*index_n == 6)continue outer;
	__str+=""+index+index_n;
    } 
}
//////////////////////////////////////////////////////////////////////////////
//CHECK#2
if (__str !== "0010112021223031") {
	throw new Test262Error('#2: __str === "0010112021223031". Actual:  __str ==='+ __str  );
}
//
//////////////////////////////////////////////////////////////////////////////

__str="";

outer : for(var index=0; index<4; index+=1) {
    nested : for(var index_n=0; index_n<=index; index_n++) {
	if (index*index_n == 6)continue ;
	__str+=""+index+index_n;
    } 
}

//////////////////////////////////////////////////////////////////////////////
//CHECK#3
if (__str !== "001011202122303133") {
	throw new Test262Error('#3: __str === "001011202122303133". Actual:  __str ==='+ __str  );
}
//
//////////////////////////////////////////////////////////////////////////////


console.log("pass");
