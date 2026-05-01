// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: "Evaluate the production ArrayLiteral: [ Elision ]"
es5id: 11.1.4_A1.2
description: >
    Checking various properties the array defined with "var array =
    [,,,,,]"
---*/

var array = [,,,,,];

//CHECK#1
console.log(!(typeof array !== "object"));

//CHECK#2
console.log(!(array instanceof Array !== true));

//CHECK#3
console.log(!(array.toString !== Array.prototype.toString));

//CHECK#4
console.log(!(array.length !== 5));

