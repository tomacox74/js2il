// Copyright (C) 2015 Jordan Harband. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-object.values
description: Object.values accepts boolean primitives.
author: Jordan Harband
---*/

var trueResult = Object.values(true);

console.log(Object.is(Array.isArray(trueResult), true));
console.log(Object.is(trueResult.length, 0));

var falseResult = Object.values(false);

console.log(Object.is(Array.isArray(falseResult), true));
console.log(Object.is(falseResult.length, 0));
