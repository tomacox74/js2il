// Copyright (C) 2011 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 13.1
description: >
    const declarations with initialisers in statement positions:
    case Expression : StatementList
---*/

var assert = function assert(condition) {
  console.log(!!condition);
};
switch (true) { case true: const x = 1; }

