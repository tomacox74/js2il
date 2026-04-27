// Copyright (C) 2013 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 12.12.3
description: >
    "Logical OR" Symbol evaluation
features: [Symbol]
---*/
var sym = Symbol();

console.log(Object.is(!sym || true, true));
console.log(Object.is(sym || false, sym));
