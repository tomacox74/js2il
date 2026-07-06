// Copyright (C) 2015 André Bargull. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
description: >
    Generator expressions cannot be used as constructors.
es6id: 14.4
features: [generators]
---*/

function __sameValue(actual, expected) {
  return Object.is(actual, expected);
}

function __assertResult(passed, message) {
  console.log(!!passed);
  if (!passed) {
    throw new Error(message || 'Assertion failed');
  }
}

var g = function*(){};

assert.throws(TypeError, function() {
  var instance = new g();
});
