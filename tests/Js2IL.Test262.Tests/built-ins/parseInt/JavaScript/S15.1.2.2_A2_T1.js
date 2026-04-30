// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator remove leading StrWhiteSpaceChar
esid: sec-parseint-string-radix
description: "StrWhiteSpaceChar :: TAB (U+0009)"
---*/

console.log(Object.is(parseInt("\u00091"), parseInt("1")));

console.log(Object.is(parseInt("\u0009\u0009-1"), parseInt("-1")));

console.log(Object.is(parseInt("	1"), parseInt("1")));

console.log(Object.is(parseInt("			1"), parseInt("1")));

console.log(Object.is(parseInt("			\u0009			\u0009-1"), parseInt("-1")));

console.log(Object.is(parseInt("\u0009"), NaN));
