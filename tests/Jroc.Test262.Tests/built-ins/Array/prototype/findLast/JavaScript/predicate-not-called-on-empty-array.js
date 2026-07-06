// Copyright (C) 2021 Microsoft. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.findlast
description: >
  Predicate is only called if this.length is > 0.
info: |
  Array.prototype.findLast ( predicate[ , thisArg ] )

  ...
  4. Let k be len - 1.
  5. Repeat, while k ≥ 0,
    ...
    c. Let testResult be ! ToBoolean(? Call(predicate, thisArg, « kValue, 𝔽(k), O »)).
  ...
  6. Return undefined.
features: [array-find-from-last]
---*/

function testWithTypedArrayConstructors(fn) {
  var ctors = [Int8Array, Uint8Array, Int16Array, Int32Array, Float32Array, Float64Array];
  for (var i = 0; i < ctors.length; i++) {
    fn(ctors[i], function(value) { return value; });
  }
}


var called = false;

var predicate = function() {
  called = true;
  return true;
};

var result = [].findLast(predicate);

assert.sameValue(called, false, '[].findLast(predicate) does not call predicate');
assert.sameValue(result, undefined, '[].findLast(predicate) returned undefined');
