// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    When "break Identifier" is evaluated, (break, empty, Identifier) is
    returned
es5id: 12.8_A4_T2
description: Using embedded and labeled loops, breaking to nested loop
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

LABEL_OUT : var x=0, y=0, xx=0, yy=0;
(function(){
LABEL_DO_LOOP : do {
    LABEL_IN : x++;
    if(x===10)return;
    LABEL_NESTED_LOOP : do {
        LABEL_IN_NESTED : xx++;
        if(xx===10)return;
        break LABEL_NESTED_LOOP;
        LABEL_IN_NESTED_2 : yy++;
    } while (0);
    
    LABEL_IN_2 : y++;
    
    function IN_DO_FUNC(){}
} while(0);

LABEL_ANOTHER_LOOP : do {
    ;
} while(0);

function OUT_FUNC(){}
})();
//////////////////////////////////////////////////////////////////////////////
//CHECK#1
if ((x!==1)&&(y!==1)&&(xx!==1)&(yy!==0)) {
	throw new Test262Error('#1: x === 1 and y === 1 and xx === 1 and yy === 0. Actual:  x==='+x+' and y==='+y+' and xx==='+xx+' and yy==='+yy );
}
//
//////////////////////////////////////////////////////////////////////////////

console.log(true);
