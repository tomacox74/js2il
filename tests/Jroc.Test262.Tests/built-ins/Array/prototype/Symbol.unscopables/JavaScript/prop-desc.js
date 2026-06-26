// Copyright (C) 2015 Mike Pennisi. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype-@@unscopables
description: >
    Property descriptor for initial value of `Symbol.unscopables` property
info: |
    This property has the attributes { [[Writable]]: false, [[Enumerable]]:
    false, [[Configurable]]: true }.
features: [Symbol.unscopables]
---*/

let descriptor = Object.getOwnPropertyDescriptor(Array.prototype, Symbol.unscopables);
console.log(typeof descriptor === "object");
console.log(descriptor.writable === false);
console.log(descriptor.enumerable === false);
console.log(descriptor.configurable === true);
