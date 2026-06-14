// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: First expression is evaluated first, and then second expression
es5id: 11.7.2_A2.4_T2
description: Checking with "throw"
---*/

//CHECK#1
var x = function () { throw "x"; };
var y = function () { throw "y"; };
try {
   x() >> y();
   throw new Error("Test262 failure");
} catch (e) {
   if (e === "y") { console.log(false); } else {
     console.log(!(e !== "x"));
   }
}
