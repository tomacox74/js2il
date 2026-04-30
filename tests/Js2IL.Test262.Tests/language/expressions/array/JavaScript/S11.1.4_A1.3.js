// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: "Evaluate the production ArrayLiteral: [ AssignmentExpression ]"
es5id: 11.1.4_A1.3
description: >
    Checking various properteis and contents of the array defined with
    "var array = [1,2,3,4,5]"
---*/

var array = [1,2,3,4,5];

//CHECK#1
console.log(!(typeof array !== "object"));

//CHECK#2
console.log(!(array instanceof Array !== true));

//CHECK#3
console.log(!(array.toString !== Array.prototype.toString));

//CHECK#4
console.log(!(array.length !== 5));

//CHECK#5
console.log(!(array[0] !== 1));

//CHECK#6
console.log(!(array[1] !== 2));

//CHECK#7
console.log(!(array[2] !== 3));

//CHECK#8
console.log(!(array[3] !== 4));

//CHECK#9
console.log(!(array[4] !== 5));

