"use strict";

// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es5id: 15.7.3-1
description: Number constructor - [[Prototype]] is the Function prototype object
---*/

console.log(Object.is(Function.prototype.isPrototypeOf(Number), true));
