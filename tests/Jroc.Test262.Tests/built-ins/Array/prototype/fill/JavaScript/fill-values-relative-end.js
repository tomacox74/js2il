// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.fill
description: >
  Fills all the elements from a with a custom start index.
info: |
  22.1.3.6 Array.prototype.fill (value [ , start [ , end ] ] )

  ...
  8. If end is undefined, let relativeEnd be len; else let relativeEnd be
  ToInteger(end).
  9. ReturnIfAbrupt(relativeEnd).
  10. If relativeEnd < 0, let final be max((len + relativeEnd),0); else let
  final be min(relativeEnd, len).
  ...
includes: [compareArray.js]
---*/

function testWithTypedArrayConstructors(fn) {
  var ctors = [Int8Array, Uint8Array, Int16Array, Int32Array, Float32Array, Float64Array];
  for (var i = 0; i < ctors.length; i++) {
    fn(ctors[i], function(value) { return value; });
  }
}


assert.compareArray([0, 0, 0].fill(8, 0, 1), [8, 0, 0],
  '[0, 0, 0].fill(8, 0, 1) must return [8, 0, 0]'
);

assert.compareArray([0, 0, 0].fill(8, 0, -1), [8, 8, 0],
  '[0, 0, 0].fill(8, 0, -1) must return [8, 8, 0]'
);

assert.compareArray([0, 0, 0].fill(8, 0, 5), [8, 8, 8],
  '[0, 0, 0].fill(8, 0, 5) must return [8, 8, 8]'
);
