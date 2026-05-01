// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es5id: 12.7-1
description: >
    The continue Statement - a continue statement without an
    identifier may have a LineTerminator before the semi-colon
---*/

function assert(value, message) {
  console.log(!!value);
}
assert.sameValue = function(actual, expected, message) {
  console.log(Object.is(actual, expected));
};
assert.notSameValue = function(actual, unexpected, message) {
  console.log(!Object.is(actual, unexpected));
};
assert.throws = function(expectedErrorConstructor, func, message) {
  try {
    func();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedErrorConstructor);
  }
};

var sum = 0;
for (var i = 1; i <= 10; i++) {
    if (true) continue
    ; else {}
    sum += i;
}

assert.sameValue(sum, 0, 'sum');

console.log(true);
