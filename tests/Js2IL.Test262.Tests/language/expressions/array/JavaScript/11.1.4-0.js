// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es5id: 11.1.4-0
description: >
    elements elided at the end of an array do not contribute to its
    length
---*/

  var a = [,];
console.log(Object.is(a.length, 1));
