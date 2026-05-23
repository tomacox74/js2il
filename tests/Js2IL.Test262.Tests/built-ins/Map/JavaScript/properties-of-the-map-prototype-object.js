// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 23.1.3
description: >
  The prototype of Map.prototype is Object.prototype.
info: |
  The Map prototype object is the intrinsic object %MapPrototype%. The value
  of the [[Prototype]] internal slot of the Map prototype object is the
  intrinsic object %ObjectPrototype% (19.1.3). The Map prototype object is an
  ordinary object. It does not have a [[MapData]] internal slot.
---*/

function assert(value) { console.log(!!value); }
assert.sameValue = function (actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function (actual, unexpected) { console.log(!Object.is(actual, unexpected)); };

assert.sameValue(
  Object.getPrototypeOf(Map.prototype),
  Object.prototype,
  'Object.getPrototypeOf(Map.prototype) returns Object.prototype'
);
