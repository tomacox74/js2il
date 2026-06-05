// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Number.MAX_VALUE is ReadOnly
es5id: 15.7.3.2_A2
description: Checking if varying Number.MAX_VALUE fails
includes: [propertyHelper.js]
---*/

function check(value) {
    console.log(value);
    if (!value) {
        throw new Error('check failed');
    }
}

var x = Number.MAX_VALUE;
var desc = Object.getOwnPropertyDescriptor(Number, 'MAX_VALUE');
check(!!desc);
check(desc.value === x);
check(desc.writable === false);
try {
    Number.MAX_VALUE = 1;
} catch (error) {
}
check(Number.MAX_VALUE === x);
