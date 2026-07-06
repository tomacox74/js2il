// Copyright (C) 2017 Jordan Harband. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
author: Jordan Harband
description: finally on a fulfilled promise can not override the resolution value
esid: sec-promise.prototype.finally
features: [Promise.prototype.finally]
flags: [async]
includes: [promiseHelper.js]
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

var sequence = [];
var obj = {};
var p = Promise.resolve(obj);

p.finally(function() {
  sequence.push(1);
  assert.sameValue(arguments.length, 0, 'onFinally receives zero args');
  return {};
}).then(function(x) {
  sequence.push(2);
  assert.sameValue(x, obj, 'onFinally can not override the resolution value');
}).then(function() {
  assert.sameValue(sequence.length, 2);
  checkSequence(sequence, "All expected callbacks called in correct order");
}).then($DONE, $DONE);
