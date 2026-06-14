// Copyright (C) 2011 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 13.6.4.13
description: >
    const ForDeclaration: creates a fresh binding per iteration
---*/

let s = '';
for (const x of [1, 2, 3]) {
  s += x;
}
console.log(Object.is(s, '123'));
