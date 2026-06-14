// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator !x returns !ToBoolean(x)
es5id: 11.4.9_A3_T1
description: Type(x) is boolean primitive or Boolean object
---*/

//CHECK#1
console.log(!(!false !== true));

//CHECK#2
console.log(!(!new Boolean(true) !== false));

//CHECK#3
console.log(!(!new Boolean(false) !== false));

