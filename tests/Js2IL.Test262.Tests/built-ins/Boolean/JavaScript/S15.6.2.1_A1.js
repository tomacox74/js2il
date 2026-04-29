"use strict";

// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    When Boolean is called as part of a new expression it is
    a constructor: it initialises the newly created object
esid: sec-boolean-constructor
description: Checking type of the newly created object and it value
---*/

console.log(Object.is(typeof new Boolean(), "object"));
console.log(!Object.is(new Boolean(), undefined));

var x3 = new Boolean();
console.log(Object.is(typeof x3, "object"));

var x4 = new Boolean();
console.log(!Object.is(x4, undefined));
console.log(Object.is(typeof new Boolean(1), "object"));
console.log(!Object.is(new Boolean(1), undefined));

var x7 = new Boolean(1);
console.log(Object.is(typeof x7, "object"));

var x8 = new Boolean(1);
console.log(!Object.is(x8, undefined));
