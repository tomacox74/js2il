// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: If x is less than -1, Math.acos(x) is NaN
es5id: 15.8.2.2_A3
description: Checking if Math.acos(x) is NaN, where x is less than -1
---*/

function check(value) {
    console.log(value);
    if (!value) {
        throw new Error('check failed');
    }
}

check(Object.is(Math.acos(-1.000000000000001), NaN));
check(Object.is(Math.acos(-2), NaN));
check(Object.is(Math.acos(-Infinity), NaN));
