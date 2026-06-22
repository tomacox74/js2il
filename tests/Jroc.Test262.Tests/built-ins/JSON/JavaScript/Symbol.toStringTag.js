// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 24.3.3
description: >
    `Symbol.toStringTag` property descriptor
info: |
    The initial value of the @@toStringTag property is the String value
    "JSON".

    This property has the attributes { [[Writable]]: false, [[Enumerable]]:
    false, [[Configurable]]: true }.
includes: [propertyHelper.js]
features: [Symbol.toStringTag]
---*/

var desc = Object.getOwnPropertyDescriptor(JSON, Symbol.toStringTag);
console.log(JSON[Symbol.toStringTag] === "JSON");
console.log(desc !== undefined);
console.log(desc !== undefined && desc.writable === false);
console.log(desc !== undefined && desc.enumerable === false);
console.log(desc !== undefined && desc.configurable === true);
