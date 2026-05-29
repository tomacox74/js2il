// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: If x is -Infinity, Math.exp(x) is +0
es5id: 15.8.2.8_A5
description: Checking if Math.exp(-Infinity) is +0
---*/

function check(value) {
    console.log(value);
    if (!value) {
        throw new Error('check failed');
    }
}

check(Object.is(Math.exp(-Infinity), 0));
