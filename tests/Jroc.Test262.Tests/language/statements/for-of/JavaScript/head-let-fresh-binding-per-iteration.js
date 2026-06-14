// Copyright (C) 2011 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 13.6.4.13
description: >
    let ForDeclaration: creates a fresh binding per iteration
---*/
// test262 execution-port helpers
var assert = function assert(condition) {
  console.log(Boolean(condition));
};

assert.sameValue = function (actual, expected) {
  console.log(Object.is(actual, expected));
};

assert.notSameValue = function (actual, unexpected) {
  console.log(!Object.is(actual, unexpected));
};

assert.throws = function (expectedError, fn) {
  var passed = false;
  try {
    fn();
  } catch (error) {
    passed = error instanceof expectedError;
  }
  console.log(passed);
};


let s = 0;
let f = [undefined, undefined, undefined];

for (let x of [1, 2, 3]) {
  s += x;
  f[x-1] = function() { return x; }
}
assert.sameValue(s, 6, "The value of `s` is `6`");
assert.sameValue(f[0](), 1, "`f[0]()` returns `1`");
assert.sameValue(f[1](), 2, "`f[1]()` returns `2`");
assert.sameValue(f[2](), 3, "`f[2]()` returns `3`");
