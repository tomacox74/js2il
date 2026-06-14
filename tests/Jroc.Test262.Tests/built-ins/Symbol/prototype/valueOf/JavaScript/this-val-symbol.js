// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-symbol.prototype.valueof
description: Called on a Symbol value
info: |
  1. Let s be the this value.
  2. If Type(s) is Symbol, return s.
features: [Symbol]
---*/

function assert(value) { console.log(!!value); }
assert.sameValue = function (actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function (actual, unexpected) { console.log(!Object.is(actual, unexpected)); };

var valueOf = Symbol.prototype.valueOf;
var subject = Symbol('s');

assert.sameValue(valueOf.call(subject), subject);
