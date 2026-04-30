// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator use ToString
esid: sec-parsefloat-string
description: Checking for boolean primitive
---*/

console.log(Object.is(parseFloat(true), NaN));
console.log(Object.is(parseFloat(false), NaN));
