// Copyright (C) 2013 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 25.2
description: >
    When a string is the operand of a `yield *` expression, the generator
    should produce an iterator that visits each character in order.
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

function* g() {
  yield* 'abc';
}
var iter = g();
var result;

result = iter.next();
assert.sameValue(result.value, 'a', 'First result `value`');
assert.sameValue(result.done, false, 'First result `done` flag');

result = iter.next();
assert.sameValue(result.value, 'b', 'Second result `value`');
assert.sameValue(result.done, false, 'Second result `done` flag');

result = iter.next();
assert.sameValue(result.value, 'c', 'Third result `value`');
assert.sameValue(result.done, false, 'Third result `done` flag');

result = iter.next();
assert.sameValue(result.value, undefined, 'Final result `value`');
assert.sameValue(result.done, true, 'Final result `done` flag');
