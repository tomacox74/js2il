// Copyright 2017 Caitlin Potter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
author: Caitlin Potter <caitp@igalia.com>
esid: pending
description: >
  Implementations must defer resolving an async function's Promise until after
  all finally blocks have been evaluated.
flags: [async]
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

async function f() {
  try {
    return "early-return";
  } finally {
    throw "override";
  }
}

f().then($DONE, function(value) {
  assert.sameValue(value, "override", "Exception thrown in finally block");
}).then($DONE, $DONE);
