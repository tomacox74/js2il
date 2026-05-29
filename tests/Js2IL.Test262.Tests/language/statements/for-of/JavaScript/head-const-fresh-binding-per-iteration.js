// Copyright (C) 2011 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 13.6.4.13
description: >
    const ForDeclaration: creates a fresh binding per iteration
---*/
function assert(value) {
  console.log(!!value);
}

assert.sameValue = function(actual, expected) {
  console.log(Object.is(actual, expected));
};

assert.notSameValue = function(actual, unexpected) {
  console.log(!Object.is(actual, unexpected));
};

assert.compareArray = function(actual, expected) {
  if (!Array.isArray(actual) || !Array.isArray(expected) || actual.length !== expected.length) {
    console.log(false);
    return;
  }

  for (let i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      console.log(false);
      return;
    }
  }

  console.log(true);
};

assert.throws = function(expectedCtor, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedCtor);
  }
};

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;


let s = 0;
let f = [undefined, undefined, undefined];

for (const x of [1, 2, 3]) {
  s += x;
  f[x-1] = function() { return x; }
}
assert.sameValue(s, 6, "The value of `s` is `6`");
assert.sameValue(f[0](), 1, "`f[0]()` returns `1`");
assert.sameValue(f[1](), 2, "`f[1]()` returns `2`");
assert.sameValue(f[2](), 3, "`f[2]()` returns `3`");
