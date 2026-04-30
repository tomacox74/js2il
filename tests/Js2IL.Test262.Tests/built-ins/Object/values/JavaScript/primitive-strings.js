// Copyright (C) 2015 Jordan Harband. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-object.values
description: Object.values accepts string primitives.
author: Jordan Harband
---*/

var result = Object.values('abc');

console.log(Object.is(Array.isArray(result), true));
console.log(Object.is(result.length, 3));

console.log(Object.is(result[0], 'a'));
console.log(Object.is(result[1], 'b'));
console.log(Object.is(result[2], 'c'));
