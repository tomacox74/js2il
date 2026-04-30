// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator use ToString
esid: sec-parseint-string-radix
description: Checking for number primitive
---*/

console.log(Object.is(parseInt(-1), parseInt("-1")));
console.log(Object.is(String(parseInt(Infinity)), "NaN"));
console.log(Object.is(String(parseInt(NaN)), "NaN"));
console.log(Object.is(parseInt(-0), 0));
