// Copyright (C) 2020 Jordan Harband. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
description: >
    `Symbol.toStringTag` property descriptor
info: |
    The initial value of the @@toStringTag property is the String value
    "Reflect".

    This property has the attributes { [[Writable]]: false, [[Enumerable]]:
    false, [[Configurable]]: true }.
includes: [propertyHelper.js]
features: [Symbol.toStringTag, Reflect]
---*/

var desc = Object.getOwnPropertyDescriptor(Reflect, Symbol.toStringTag);
console.log(Reflect[Symbol.toStringTag] === "Reflect");
console.log(desc !== undefined);
console.log(desc !== undefined && desc.writable === false);
console.log(desc !== undefined && desc.enumerable === false);
console.log(desc !== undefined && desc.configurable === true);
