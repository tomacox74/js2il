// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es5id: 12.14-9
description: catch introduces scope - name lookup finds outer variable
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

  function f(o) {
    var x = 42;

    function innerf(o) {
      try {
        throw o;
      }
      catch (e) {
        return x;
      }
    }

    return innerf(o);
  }

assert.sameValue(f({}), 42);

console.log(true);
