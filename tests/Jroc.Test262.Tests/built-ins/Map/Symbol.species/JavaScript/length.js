// Copyright (C) 2015 André Bargull. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es6id: 23.1.2.2
description: >
  get Map [ @@species ].length is 0.
features: [Symbol.species]
---*/

var desc = Object.getOwnPropertyDescriptor(Map, Symbol.species);
var getterLengthDesc = Object.getOwnPropertyDescriptor(desc.get, "length");

console.log(getterLengthDesc.value === 0);
console.log(getterLengthDesc.writable === false);
console.log(getterLengthDesc.enumerable === false);
console.log(getterLengthDesc.configurable === true);
