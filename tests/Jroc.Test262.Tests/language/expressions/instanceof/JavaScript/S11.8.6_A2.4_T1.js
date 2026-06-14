// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: First expression is evaluated first, and then second expression
es5id: 11.8.6_A2.4_T1
description: Checking with "="
---*/

var OBJECT = 0;
console.log(Object.is((OBJECT = Object, {}) instanceof OBJECT, true));

var object = {};
console.log(Object.is(object instanceof (object = 0, Object), true));
