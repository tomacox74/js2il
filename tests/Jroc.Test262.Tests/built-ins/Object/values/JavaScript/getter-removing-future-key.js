// Copyright (C) 2015 Jordan Harband. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-object.values
description: Object.values does not see an element removed by a getter that is hit during iteration
author: Jordan Harband
---*/

var bDeletesC = {
  a: 'A',
  get b() {
    delete this.c;
    return 'B';
  },
  c: 'C'
};

var result = Object.values(bDeletesC);

console.log(Object.is(Array.isArray(result), true));
console.log(Object.is(result.length, 2));

console.log(Object.is(result[0], 'A'));
console.log(Object.is(result[1], 'B'));
