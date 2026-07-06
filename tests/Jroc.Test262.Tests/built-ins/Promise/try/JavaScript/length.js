// Copyright (C) 2024 Jordan Harband. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
description: Promise.try `length` property
features: [promise-try]
---*/

function sameValue(actual, expected) {
    return Object.is(actual, expected);
}

verifyProperty(Promise.try, "length", {
    value: 1,
    writable: false,
    enumerable: false,
    configurable: true
});
