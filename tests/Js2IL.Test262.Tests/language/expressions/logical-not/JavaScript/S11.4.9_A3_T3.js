// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator !x returns !ToBoolean(x)
es5id: 11.4.9_A3_T3
description: Type(x) is string primitive or String object
---*/

console.log(Object.is(!"1", false));
console.log(Object.is(!new String("0"), false));
console.log(Object.is(!"x", false));
console.log(Object.is(!"", true));
console.log(Object.is(!new String(""), false));
