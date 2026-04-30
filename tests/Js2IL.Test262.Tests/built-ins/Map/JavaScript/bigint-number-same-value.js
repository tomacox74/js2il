"use strict";

// Copyright (C) 2021 Rick Waldron. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-map.prototype.set
description: >
  Observing the expected behavior of keys when a BigInt and Number have
  the same value.
info: |
  Map.prototype.set ( key , value )

  ...
  Let p be the Record {[[key]]: key, [[value]]: value}.
  Append p as the last element of entries.
  ...

features: [BigInt]
---*/

const number = 9007199254740991;
const bigint = 9007199254740991n;

const m = new Map([
  [number, number],
  [bigint, bigint],
]);

console.log(Object.is(m.size, 2));
console.log(Object.is(m.has(number), true));
console.log(Object.is(m.has(bigint), true));

console.log(Object.is(m.get(number), number));
console.log(Object.is(m.get(bigint), bigint));

m.delete(number);
console.log(Object.is(m.size, 1));
console.log(Object.is(m.has(number), false));
m.delete(bigint);
console.log(Object.is(m.size, 0));
console.log(Object.is(m.has(bigint), false));

m.set(number, number);
console.log(Object.is(m.size, 1));
m.set(bigint, bigint);
console.log(Object.is(m.size, 2));
