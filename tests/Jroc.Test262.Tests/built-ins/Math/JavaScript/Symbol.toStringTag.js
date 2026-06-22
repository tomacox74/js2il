// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 20.2.1.9
description: >
    `Symbol.toStringTag` property descriptor
info: |
    The initial value of the @@toStringTag property is the String value
    "Math".

    This property has the attributes { [[Writable]]: false, [[Enumerable]]:
    false, [[Configurable]]: true }.
includes: [propertyHelper.js]
features: [Symbol.toStringTag]
---*/

var desc = Object.getOwnPropertyDescriptor(Math, Symbol.toStringTag);
console.log(Math[Symbol.toStringTag] === "Math");
console.log(desc !== undefined);
console.log(desc !== undefined && desc.writable === false);
console.log(desc !== undefined && desc.enumerable === false);
console.log(desc !== undefined && desc.configurable === true);
