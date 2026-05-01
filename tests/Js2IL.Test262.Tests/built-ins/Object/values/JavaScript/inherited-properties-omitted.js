// Copyright (C) 2015 Jordan Harband. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-object.values
description: Object.values does not see inherited properties.
author: Jordan Harband
---*/

var F = function G() {};
F.prototype.a = {};
F.prototype.b = {};

var f = new F();
f.b = {}; // shadow the prototype
f.c = {}; // solely an own property

var result = Object.values(f);

console.log(Object.is(Array.isArray(result), true));
console.log(Object.is(result.length, 2));

console.log(Object.is(result[0], f.b));
console.log(Object.is(result[1], f.c));
