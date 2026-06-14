// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The production TryStatement: "try Block Finally" and the production
    TryStatement: "try Block Catch Finally"
es5id: 12.14_A5
description: Checking "catch" catches the Identifier in appropriate way
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

// CHECK#1
try {
  throw "catchme";	
  throw "dontcatchme";
  throw new Test262Error('#1.1: throw "catchme" lead to throwing exception');
}
catch (e) {
  if(e==="dontcatchme"){
    throw new Test262Error('#1.2: Exception !== "dontcatchme"');
  }
  if (e!=="catchme") {
    throw new Test262Error('#1.3: Exception === "catchme". Actual:  Exception ==='+ e  );
  }
}

// CHECK#2
function SwitchTest1(value){
  var result = 0;
  try{  
    switch(value) {
      case 1:
        result += 4;
        throw result;
        break;
      case 4:
        result += 64;
        throw "ex";
    }
  return result;
  }
  catch(e){	
    if ((value===1)&&(e!==4)) throw new Test262Error('#2.1: Exception === 4. Actual: '+e);
    if ((value===4)&&(e!=="ex"))throw new Test262Error('#2.2: Exception === "ex". Actual: '+e);
  }
  finally{
    return result;
  }
}
if (SwitchTest1(1)!==4) throw new Test262Error('#2.3: "finally" block must be evaluated');
if (SwitchTest1(4)!==64)throw new Test262Error('#2.4: "finally" block must be evaluated');

console.log(true);
