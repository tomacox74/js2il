// Copyright (C) 2013 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 25.2
description: >
    When the `next` method of a generator-produced iterable is invoked without
    an argument, the corresponding `yield` expression should be evaluated as
    `undefined`.
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

function* g() { actual = yield; }
var iter = g();
var actual, result;

result = iter.next();
assert.sameValue(result.value, undefined);
assert.sameValue(result.done, false);
assert.sameValue(actual, undefined);

result = iter.next();
assert.sameValue(result.value, undefined);
assert.sameValue(result.done, true);
assert.sameValue(actual, undefined);
