// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: If x is NaN, Math.atan(x) is NaN
es5id: 15.8.2.4_A1
description: Checking if Math.atan(NaN) is NaN
---*/

function check(value) {
    console.log(value);
    if (!value) {
        throw new Error('check failed');
    }
}

check(Object.is(Math.atan(NaN), NaN));
