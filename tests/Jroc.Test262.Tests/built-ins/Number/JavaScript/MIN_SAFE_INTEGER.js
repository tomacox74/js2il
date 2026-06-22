// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
description: Property descriptor for `Number.MIN_SAFE_INTEGER`
esid: sec-number.min_safe_integer
info: |
    The value of Number.MIN_SAFE_INTEGER is -9007199254740991

    This property has the attributes { [[Writable]]: false, [[Enumerable]]:
    false, [[Configurable]]: false }.
includes: [propertyHelper.js]
---*/

var desc = Object.getOwnPropertyDescriptor(Number, 'MIN_SAFE_INTEGER');

console.log(desc !== undefined);
console.log(desc !== undefined && desc.set === undefined);
console.log(desc !== undefined && desc.get === undefined);
console.log(desc !== undefined && desc.value === -9007199254740991);
console.log(desc !== undefined && desc.enumerable === false);
console.log(desc !== undefined && desc.writable === false);
console.log(desc !== undefined && desc.configurable === false);
