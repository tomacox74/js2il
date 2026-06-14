// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 23.1.4
description: >
  Map instances are ordinary objects that inherit properties from the Map
  prototype.
---*/

function assert(value) { console.log(!!value); }
assert.sameValue = function (actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function (actual, unexpected) { console.log(!Object.is(actual, unexpected)); };

assert.sameValue(
  Object.getPrototypeOf(new Map()),
  Map.prototype,
  '`Object.getPrototypeOf(new Map())` returns `Map.prototype`'
);
