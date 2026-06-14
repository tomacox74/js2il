// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-array.isarray
description: Array.isArray return false if its argument is not an Array
---*/

console.log(Object.is(Array.isArray(42), false));
console.log(Object.is(Array.isArray(undefined), false));
console.log(Object.is(Array.isArray(true), false));
console.log(Object.is(Array.isArray("abc"), false));
console.log(Object.is(Array.isArray({}), false));
console.log(Object.is(Array.isArray(null), false));
