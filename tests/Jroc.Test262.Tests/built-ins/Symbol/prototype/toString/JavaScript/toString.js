// Copyright (C) 2013 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 19.4.3.2
description: >
    toString operations on Symbols
features: [Symbol]
---*/

function assert(value) { console.log(!!value); }
assert.sameValue = function (actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function (actual, unexpected) { console.log(!Object.is(actual, unexpected)); };

assert.sameValue(
  String(Symbol('66')),
  'Symbol(66)',
  "`String(Symbol('66'))` returns `'Symbol(66)'`"
)
assert.sameValue(
  Symbol('66').toString(),
  'Symbol(66)',
  "`Symbol('66').toString()` returns `'Symbol(66)'`"
);
assert.sameValue(
  Object(Symbol('66')).toString(),
  'Symbol(66)',
  "`Object(Symbol('66')).toString()` returns `'Symbol(66)'`"
);
assert.sameValue(
  Symbol.prototype.toString.call(Symbol('66')),
  'Symbol(66)',
  "`Symbol.prototype.toString.call(Symbol('66'))` returns `'Symbol(66)'`"
);
