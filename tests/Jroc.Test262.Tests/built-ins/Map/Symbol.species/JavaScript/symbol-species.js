// Copyright 2015 Cubane Canada, Inc.  All rights reserved.
// See LICENSE for details.

/*---
info: |
  Map has a property at `Symbol.species`
esid: sec-get-map-@@species
description: Map[Symbol.species] exists per spec
features: [Symbol.species]
---*/

var desc = Object.getOwnPropertyDescriptor(Map, Symbol.species);

console.log(desc.set === undefined);
console.log(typeof desc.get === "function");
console.log(desc.enumerable === false);
console.log(desc.configurable === true);
