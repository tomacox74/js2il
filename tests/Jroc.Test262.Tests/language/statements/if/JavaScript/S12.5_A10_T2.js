// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Function expession inside the "if" expression is allowed
es5id: 12.5_A10_T2
description: >
    Using function expession "function __func(){return 0;}()" within
    "if" expression
---*/

//////////////////////////////////////////////////////////////////////////////
//CHECK#
var Test262Error = function(message) {
  this.name = 'Test262Error';
  this.message = message || '';
};
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

if(function __func(){return 0;}()){
    throw new Test262Error('#1: Function expession inside the if expression is allowed');
}else {
    ;
}
//
//////////////////////////////////////////////////////////////////////////////

console.log(true);
