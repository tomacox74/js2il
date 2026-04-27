// Copyright 2015 Microsoft Corporation. All rights reserved.
// This code is governed by the license found in the LICENSE file.

/*---
description: >
  Test override of Object.Assign(target,...sources),
  Every string from sources will be wrapped to objects, and override from the first letter(result[0]) all the time
es6id:  19.1.2.1
---*/

var target = 12;
var result = Object.assign(target, "aaa", "bb2b", "1c");

console.log(Object.is(Object.getOwnPropertyNames(result).length, 4));
console.log(Object.is(result[0], "1"));
console.log(Object.is(result[1], "c"));
console.log(Object.is(result[2], "2"));
console.log(Object.is(result[3], "b"));
