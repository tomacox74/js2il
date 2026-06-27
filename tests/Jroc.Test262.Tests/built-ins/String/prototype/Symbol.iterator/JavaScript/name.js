// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 25.1.3.27
description: Descriptor for `name` property
info: |
  The value of the name property of this function is "[Symbol.iterator]".
features: [Symbol.iterator]
---*/

let descriptor = Object.getOwnPropertyDescriptor(String.prototype[Symbol.iterator], "name");
console.log(descriptor.value === "[Symbol.iterator]");
console.log(descriptor.writable === false);
console.log(descriptor.enumerable === false);
console.log(descriptor.configurable === true);
