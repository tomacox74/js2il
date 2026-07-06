// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: String.prototype.charAt(pos)
es5id: 15.5.4.4_A1_T6
description: >
    Call charAt() function with x argument of new String object, where
    x is undefined variable
---*/

if (new String("lego").charAt(x) !== "l") {
  throw new Test262Error('#1: var x; new String("lego").charAt(x) === "l". Actual: new String("lego").charAt(x) ===' + new String("lego").charAt(x));
}
//
//////////////////////////////////////////////////////////////////////////////

var x;
