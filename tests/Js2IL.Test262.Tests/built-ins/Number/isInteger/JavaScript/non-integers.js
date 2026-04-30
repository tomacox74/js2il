// Copyright (c) 2014 Ryan Lewis. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es6id: 20.1.2.3
author: Ryan Lewis
description: Number.isInteger should return false if called with a double.
---*/

console.log(Object.is(Number.isInteger(6.75), false));
console.log(Object.is(Number.isInteger(0.000001), false));
console.log(Object.is(Number.isInteger(-0.000001), false));
console.log(Object.is(Number.isInteger(11e-1), false));
