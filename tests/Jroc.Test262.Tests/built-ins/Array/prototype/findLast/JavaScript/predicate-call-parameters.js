// Copyright (C) 2021 Microsoft. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.findlast
description: >
  Predicate called as F.call( thisArg, kValue, k, O ) for each array entry.
info: |
  Array.prototype.findLast ( predicate[ , thisArg ] )

  ...
  4. Let k be len - 1.
  5. Repeat, while k ≥ 0,
    ...
    c. Let testResult be ! ToBoolean(? Call(predicate, thisArg, « kValue, 𝔽(k), O »)).
    d. If testResult is true, return kValue.
  ...
features: [array-find-from-last]
---*/

function testWithTypedArrayConstructors(fn) {
  var ctors = [Int8Array, Uint8Array, Int16Array, Int32Array, Float32Array, Float64Array];
  for (var i = 0; i < ctors.length; i++) {
    fn(ctors[i], function(value) { return value; });
  }
}


var arr = ['Mike', 'Rick', 'Leo'];

var results = [];

arr.findLast(function() {
  results.push(arguments);
});

assert.sameValue(results.length, 3);

var result = results[0];
assert.sameValue(result[0], 'Leo');
assert.sameValue(result[1], 2);
assert.sameValue(result[2], arr);
assert.sameValue(result.length, 3);

result = results[1];
assert.sameValue(result[0], 'Rick');
assert.sameValue(result[1], 1);
assert.sameValue(result[2], arr);
assert.sameValue(result.length, 3);

result = results[2];
assert.sameValue(result[0], 'Mike');
assert.sameValue(result[1], 0);
assert.sameValue(result[2], arr);
assert.sameValue(result.length, 3);
