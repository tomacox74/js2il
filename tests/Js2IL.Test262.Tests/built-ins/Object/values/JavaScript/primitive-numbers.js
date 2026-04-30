// Copyright (C) 2015 Jordan Harband. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-object.values
description: Object.values accepts number primitives.
author: Jordan Harband
---*/

console.log(Object.is(Object.values(0).length, 0));
console.log(Object.is(Object.values(-0).length, 0));
console.log(Object.is(Object.values(Infinity).length, 0));
console.log(Object.is(Object.values(-Infinity).length, 0));
console.log(Object.is(Object.values(NaN).length, 0));
console.log(Object.is(Object.values(Math.PI).length, 0));
