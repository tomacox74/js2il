// Copyright (C) 2024 Jordan Harband. All rights reserved.
// See LICENSE for details.
/*---
author: Jordan Harband
description: Promise.try property descriptor
features: [promise-try]
---*/

function sameValue(actual, expected) {
    return Object.is(actual, expected);
}

function assert(condition, message) {
    var passed = !!condition;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
}

function verifyProperty(object, name, desc) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    assert(!!actual, "Property descriptor missing: " + name);

    if (Object.prototype.hasOwnProperty.call(desc, "value")) {
        assert(sameValue(actual.value, desc.value), "Unexpected value for " + name);
    }

    if (Object.prototype.hasOwnProperty.call(desc, "writable")) {
        assert(actual.writable === desc.writable, "Unexpected writable for " + name);
    }

    if (Object.prototype.hasOwnProperty.call(desc, "enumerable")) {
        assert(actual.enumerable === desc.enumerable, "Unexpected enumerable for " + name);
    }

    if (Object.prototype.hasOwnProperty.call(desc, "configurable")) {
        assert(actual.configurable === desc.configurable, "Unexpected configurable for " + name);
    }
}

verifyProperty(Promise, "try", {
    value: Promise.try,
    writable: true,
    enumerable: false,
    configurable: true
});
