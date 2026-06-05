// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: If x is greater than 1, Math.asin(x) is NaN
es5id: 15.8.2.3_A2
description: Checking if Math.asin(x) is NaN, where x is greater than 1
---*/

function check(value) {
    console.log(value);
    if (!value) {
        throw new Error('check failed');
    }
}

check(Object.is(Math.asin(1.000000000000001), NaN));
check(Object.is(Math.asin(2), NaN));
check(Object.is(Math.asin(Infinity), NaN));
