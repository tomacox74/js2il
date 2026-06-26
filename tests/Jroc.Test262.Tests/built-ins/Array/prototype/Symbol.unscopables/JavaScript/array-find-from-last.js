// Copyright (C) 2022 Microsoft. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype-@@unscopables
description: >
    Initial value of `Symbol.unscopables` property includes findLast and findLastIndex
features: [Symbol.unscopables, array-find-from-last]
---*/

let unscopables = Array.prototype[Symbol.unscopables];
console.log(Object.getPrototypeOf(unscopables) === null);

let findLastDescriptor = Object.getOwnPropertyDescriptor(unscopables, "findLast");
console.log(unscopables.findLast === true);
console.log(typeof findLastDescriptor === "object");
console.log(findLastDescriptor.writable === true);
console.log(findLastDescriptor.enumerable === true);
console.log(findLastDescriptor.configurable === true);

let findLastIndexDescriptor = Object.getOwnPropertyDescriptor(unscopables, "findLastIndex");
console.log(unscopables.findLastIndex === true);
console.log(typeof findLastIndexDescriptor === "object");
console.log(findLastIndexDescriptor.writable === true);
console.log(findLastIndexDescriptor.enumerable === true);
console.log(findLastIndexDescriptor.configurable === true);
