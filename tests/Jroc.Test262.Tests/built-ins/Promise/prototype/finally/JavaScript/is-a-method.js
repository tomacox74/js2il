// Copyright (C) 2017 Jordan Harband. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
author: Jordan Harband
description: finally is a method on a Promise
esid: sec-promise.prototype.finally
features: [Promise.prototype.finally]
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

function checkSequence(sequence, message) {
  var passed = true;
  for (var i = 0; i < sequence.length; i++) {
    if (sequence[i] !== i + 1) {
      passed = false;
      break;
    }
  }
  __assertResult(passed, message || 'Unexpected callback sequence');
}

var p = Promise.resolve(3);

assert.sameValue(
  p.finally,
  Promise.prototype.finally,
  'Expected the `finally` method on a Promise to be `Promise.prototype.finally`'
);
