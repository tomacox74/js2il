// Copyright (C) 2015 Mike Pennisi. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype-@@unscopables
description: >
    Initial value of `Symbol.unscopables` property
features: [Symbol.unscopables]
---*/

let unscopables = Array.prototype[Symbol.unscopables];
console.log(Object.getPrototypeOf(unscopables) === null);

function verifyUnscopable(name) {
  let descriptor = Object.getOwnPropertyDescriptor(unscopables, name);
  console.log(unscopables[name] === true);
  console.log(typeof descriptor === "object");
  console.log(descriptor.writable === true);
  console.log(descriptor.enumerable === true);
  console.log(descriptor.configurable === true);
}

verifyUnscopable("copyWithin");
verifyUnscopable("entries");
verifyUnscopable("fill");
verifyUnscopable("find");
verifyUnscopable("findIndex");
verifyUnscopable("flat");
verifyUnscopable("flatMap");
verifyUnscopable("includes");
verifyUnscopable("keys");
verifyUnscopable("values");
