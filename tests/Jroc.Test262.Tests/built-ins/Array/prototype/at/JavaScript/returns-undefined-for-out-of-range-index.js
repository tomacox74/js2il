// Copyright (C) 2020 Rick Waldron. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.at
description: >
  Returns undefined if the specified index less than or greater than the available index range.
info: |
  Array.prototype.at( index )

  If k < 0 or k ≥ len, then return undefined.
features: [Array.prototype.at]
---*/

console.log(typeof Array.prototype.at === 'function');

let a = [];

console.log(a.at(-2) === undefined);
console.log(a.at(0) === undefined);
console.log(a.at(1) === undefined);
