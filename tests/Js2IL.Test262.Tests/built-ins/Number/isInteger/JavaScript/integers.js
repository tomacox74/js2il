// Copyright (c) 2014 Ryan Lewis. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es6id: 20.1.2.3
author: Ryan Lewis
description: Number.isInteger should return true if called with an integer.
---*/

console.log(Object.is(Number.isInteger(478), true));
console.log(Object.is(Number.isInteger(-0), true));
console.log(Object.is(Number.isInteger(0), true));
console.log(Object.is(Number.isInteger(-1), true));
console.log(Object.is(Number.isInteger(9007199254740991), true));
console.log(Object.is(Number.isInteger(-9007199254740991), true));
console.log(Object.is(Number.isInteger(9007199254740992), true));
console.log(Object.is(Number.isInteger(-9007199254740992), true));
