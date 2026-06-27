// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es6id: 23.1.2.2
description: >
  Map[Symbol.species] accessor property get name
features: [Symbol.species]
---*/

var descriptor = Object.getOwnPropertyDescriptor(Map, Symbol.species);

console.log(descriptor.get.name === "get [Symbol.species]");
