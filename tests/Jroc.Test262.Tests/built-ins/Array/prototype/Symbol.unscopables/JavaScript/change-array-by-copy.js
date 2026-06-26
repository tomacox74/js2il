// Copyright (C) 2021 Igalia. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype-@@unscopables
description: >
    Initial value of `Symbol.unscopables` property includes copy-by-change methods
features: [Symbol.unscopables, change-array-by-copy]
---*/

let unscopables = Array.prototype[Symbol.unscopables];

function verifyUnscopable(name) {
  let descriptor = Object.getOwnPropertyDescriptor(unscopables, name);
  console.log(unscopables[name] === true);
  console.log(typeof descriptor === "object");
  console.log(descriptor.writable === true);
  console.log(descriptor.configurable === true);
}

verifyUnscopable("toReversed");
verifyUnscopable("toSorted");
verifyUnscopable("toSpliced");
console.log(Object.prototype.hasOwnProperty.call(unscopables, "with") === false);
