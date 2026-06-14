// Copyright (C) 2014 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 12.2.8
description: Expressions should be evaluated in left-to-right order.
---*/

var tag = function(templateObject, a, b, c) {
  callCount++;
  console.log(Object.is(a, 0));
  console.log(Object.is(b, 1));
  console.log(Object.is(c, 2));
};
var i = 0;
var callCount;

console.log(Object.is(`a${ i++ }b${ i++ }c${ i++ }d`, 'a0b1c2d'));

i = 0;
callCount = 0;

tag`a${ i++ }b${ i++ }c${ i++ }d`;

console.log(Object.is(callCount, 1));
