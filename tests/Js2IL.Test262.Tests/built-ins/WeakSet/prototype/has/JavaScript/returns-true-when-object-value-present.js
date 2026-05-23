// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-weakset.prototype.has
description: >
  Returns true when an Object value is present in the WeakSet entries list.
info: |
  WeakSet.prototype.has ( _value_ )
  5. For each element _e_ of _entries_, do
    a. If _e_ is not ~empty~ and SameValue(_e_, _value_) is *true*, return *true*.
features: [WeakSet]
---*/

function assert(value) { console.log(!!value); }
assert.sameValue = function (actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function (actual, unexpected) { console.log(!Object.is(actual, unexpected)); };

var foo = {};
var s = new WeakSet();

s.add(foo);
assert.sameValue(s.has(foo), true);
