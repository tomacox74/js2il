// Copyright (C) 2020 Rick Waldron. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.at
description: >
  Return abrupt from ToObject(this value).
info: |
  Array.prototype.at( index )

  Let O be ? ToObject(this value).

features: [Array.prototype.at]
---*/

console.log(typeof Array.prototype.at === 'function');

try {
  Array.prototype.at.call(undefined);
  console.log(false);
} catch (e) {
  console.log(e instanceof TypeError);
}

try {
  Array.prototype.at.call(null);
  console.log(false);
} catch (e) {
  console.log(e instanceof TypeError);
}
