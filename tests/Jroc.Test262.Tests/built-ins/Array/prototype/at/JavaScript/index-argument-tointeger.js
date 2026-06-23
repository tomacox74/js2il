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

let valueOfCallCount = 0;
let index = {
  valueOf() {
    valueOfCallCount++;
    return 1;
  }
};

let a = [0, 1, 2, 3];

console.log(a.at(index) === 1);
console.log(valueOfCallCount === 1);
