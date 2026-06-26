// Copyright (C) 2020 Rick Waldron. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-ecmascript-standard-built-in-objects
description: >
  Array.prototype[Symbol.iterator] does not implement [[Construct]], is not new-able
features: [Reflect.construct, Symbol, Symbol.iterator, arrow-function]
---*/

let threwTypeError = false;
try {
  new Array.prototype[Symbol.iterator]();
} catch (error) {
  threwTypeError = error && error.name === "TypeError";
}

console.log(threwTypeError);
