// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator remove leading StrWhiteSpaceChar
esid: sec-parsefloat-string
description: "StrWhiteSpaceChar :: TAB (U+0009)"
---*/

console.log(Object.is(parseFloat("\u00091.1"), parseFloat("1.1")));
console.log(Object.is(parseFloat("\u0009\u0009-1.1"), parseFloat("-1.1")));
console.log(Object.is(parseFloat("	1.1"), parseFloat("1.1")));
console.log(Object.is(parseFloat("			1.1"), parseFloat("1.1")));
console.log(Object.is(parseFloat("			\u0009			\u0009-1.1"), parseFloat("-1.1")));
console.log(Object.is(parseFloat("\u0009"), NaN));
