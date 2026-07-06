// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-%typedarray%.prototype.values
description: >
  The prototype of the returned iterator is ArrayIteratorPrototype
info: |
  22.2.3.30 %TypedArray%.prototype.values ( )

  ...
  3. Return CreateArrayIterator(O, "value").
includes: [testTypedArray.js]
features: [Symbol.iterator, TypedArray]
---*/


var TypedArray = Object.getPrototypeOf(Int8Array);

function testWithTypedArrayConstructors(fn) {
  var ctors = [Int8Array, Uint8Array, Int16Array, Int32Array, Float32Array, Float64Array];
  for (var i = 0; i < ctors.length; i++) {
    fn(ctors[i], function(value) { return value; });
  }
}

var ArrayIteratorProto = Object.getPrototypeOf([][Symbol.iterator]());

testWithTypedArrayConstructors(function(TA, makeCtorArg) {
  var sample = new TA(makeCtorArg([0, 42, 64]));
  var iter = sample.values();

  assert.sameValue(Object.getPrototypeOf(iter), ArrayIteratorProto);
});
