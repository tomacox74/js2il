// Copyright (C) 2020 Rick Waldron. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.at
description: >
  Property type and descriptor.
info: |
  Array.prototype.at( index )

  Let relativeIndex be ? ToInteger(index).

features: [Array.prototype.at]
---*/

console.log(typeof Array.prototype.at === 'function');

let a = [0, 1, 2, 3];

console.log(a.at(false) === 0);
console.log(a.at(null) === 0);
console.log(a.at(undefined) === 0);
console.log(a.at("") === 0);
console.log(a.at(function() {}) === 0);
console.log(a.at([]) === 0);

console.log(a.at(true) === 1);
console.log(a.at("1") === 1);
