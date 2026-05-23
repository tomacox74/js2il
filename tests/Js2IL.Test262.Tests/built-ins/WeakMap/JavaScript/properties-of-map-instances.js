// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-properties-of-weakmap-instances
description: >
  WeakMap instances are ordinary objects that inherit properties from the
  WeakMap prototype.
---*/

function assert(value) { console.log(!!value); }
assert.sameValue = function (actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function (actual, unexpected) { console.log(!Object.is(actual, unexpected)); };

assert.sameValue(
  Object.getPrototypeOf(new WeakMap()),
  WeakMap.prototype,
  '`Object.getPrototypeOf(new WeakMap())` returns `WeakMap.prototype`'
);
