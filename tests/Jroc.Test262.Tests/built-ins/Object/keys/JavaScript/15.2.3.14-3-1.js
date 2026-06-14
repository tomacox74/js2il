// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es5id: 15.2.3.14-3-1
description: >
    Object.keys returns the standard built-in Array containing own
    enumerable properties
---*/

var o = {
  x: 1,
  y: 2
};

var a = Object.keys(o);

console.log(Object.is(a.length, 2));
console.log(Object.is(a[0], 'x'));
console.log(Object.is(a[1], 'y'));
