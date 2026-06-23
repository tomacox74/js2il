// Copyright (C) 2020 Rick Waldron. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.at
description: >
  Returns the item value at the specified relative index
info: |
  Array.prototype.at ( )

  Let O be ? ToObject(this value).
  Let len be ? LengthOfArrayLike(O).
  Let relativeIndex be ? ToInteger(index).
  If relativeIndex ≥ 0, then
    Let k be relativeIndex.
  Else,
    Let k be len + relativeIndex.
  If k < 0 or k ≥ len, then return undefined.
  Return ? Get(O, ! ToString(k)).

features: [Array.prototype.at]
---*/

console.log(typeof Array.prototype.at === 'function');

let a = [1, 2, 3, 4, , 5];

console.log(a.at(0) === 1);
console.log(a.at(-1) === 5);
console.log(a.at(-2) === undefined);
console.log(a.at(-3) === 4);
console.log(a.at(-4) === 3);
