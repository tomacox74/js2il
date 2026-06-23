// Copyright (C) 2020 Rick Waldron. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.at
description: >
  Array.prototype.at.length value and descriptor.
info: |
  Array.prototype.at( index )

  17 ECMAScript Standard Built-in Objects

includes: [propertyHelper.js]
features: [Array.prototype.at]
---*/

console.log(typeof Array.prototype.at === 'function');

let descriptor = Object.getOwnPropertyDescriptor(Array.prototype.at, "length");
console.log(descriptor.value === 1);
console.log(descriptor.writable === false);
console.log(descriptor.enumerable === false);
console.log(descriptor.configurable === true);
