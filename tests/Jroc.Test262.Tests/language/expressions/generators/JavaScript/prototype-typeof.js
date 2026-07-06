// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 25.2.4.2
description: >
    Whenever a GeneratorFunction instance is created another ordinary object is
    also created and is the initial value of the generator function’s prototype
    property.
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

assert.sameValue(typeof function*() {}.prototype, 'object');
