// Copyright (C) 2024 André Bargull. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-runtime-semantics-propertydestructuringassignmentevaluation
description: >
  Ensure correct evaluation order for binding lookups when destructuring target is var-binding.
info: |
  13.15.5.3 Runtime Semantics: PropertyDestructuringAssignmentEvaluation

    AssignmentProperty : PropertyName : AssignmentElement

    1. Let name be ? Evaluation of PropertyName.
    2. Perform ? KeyedDestructuringAssignmentEvaluation of AssignmentElement with arguments value and name.
    ...

  13.15.5.6 Runtime Semantics: KeyedDestructuringAssignmentEvaluation

    AssignmentElement : DestructuringAssignmentTarget Initializer_opt

    1. If DestructuringAssignmentTarget is neither an ObjectLiteral nor an ArrayLiteral, then
      a. Let lRef be ? Evaluation of DestructuringAssignmentTarget.
    2. Let v be ? GetV(value, propertyName).
    3. If Initializer is present and v is undefined, then
      ...
      b. Else,
        i. Let defaultValue be ? Evaluation of Initializer.
        ii. Let rhsValue be ? GetValue(defaultValue).
    ...
    6. Return ? PutValue(lRef, rhsValue).

includes: [compareArray.js]
features: [Proxy]
flags: [noStrict]
---*/
function Test262Error(message) {
    this.name = 'Test262Error';
    this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

function $ERROR(message) {
    throw new Test262Error(message);
}

function $DONE(error) {
    if (error) {
        throw error;
    }
}

function __test262SameValue(actual, expected) {
    return Object.is(actual, expected);
}

function __test262FormatMessage(message, fallback) {
    return message || fallback || "Assertion failed";
}

function assert(condition, message) {
    var passed = !!condition;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message));
    }
}

assert.sameValue = function(actual, expected, message) {
    var passed = __test262SameValue(actual, expected);
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Expected SameValue"));
    }
};

assert.notSameValue = function(actual, unexpected, message) {
    var passed = !__test262SameValue(actual, unexpected);
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Expected values to differ"));
    }
};

assert.throws = function(expectedErrorConstructor, fn, message) {
    var passed = false;
    try {
        fn();
    } catch (error) {
        passed = error instanceof expectedErrorConstructor ||
            (error && error.constructor === expectedErrorConstructor) ||
            (error && expectedErrorConstructor && error.name === expectedErrorConstructor.name);
    }
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Expected function to throw"));
    }
};

function compareArray(actual, expected) {
    if (!actual || !expected || actual.length !== expected.length) {
        return false;
    }

    for (var i = 0; i < actual.length; i++) {
        if (!__test262SameValue(actual[i], expected[i])) {
            return false;
        }
    }

    return true;
}

assert.compareArray = function(actual, expected, message) {
    var passed = compareArray(actual, expected);
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Expected arrays to match"));
    }
};

function verifyProperty(object, name, desc) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual;

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'value')) {
        passed = __test262SameValue(actual.value, desc.value);
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'writable')) {
        passed = actual.writable === desc.writable;
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'enumerable')) {
        passed = actual.enumerable === desc.enumerable;
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'configurable')) {
        passed = actual.configurable === desc.configurable;
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'get')) {
        passed = actual.get === desc.get;
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'set')) {
        passed = actual.set === desc.set;
    }

    console.log(passed);
    if (!passed) {
        throw new Error('verifyProperty failed for ' + name);
    }
}
var log = [];

var targetKey = {
  toString: () => {
    log.push("targetKey");
    return "q";
  }
};

var sourceKey = {
  toString: () => {
    log.push("sourceKey");
    return "p";
  }
};

var source = {
  get p() {
    log.push("get source");
    return undefined;
  }
};

var target = {
  set q(v) {
    log.push("set target");
  },
};

var env = new Proxy({}, {
  has(t, pk) {
    log.push("binding::" + pk);
  }
});

var defaultValue = 0;

with (env) {
  ({
    [sourceKey]: target[targetKey] = defaultValue
  } = source);
}

assert.compareArray(log, [
  "binding::source",
  "binding::sourceKey",
  "sourceKey",
  "binding::target",
  "binding::targetKey",
  "get source",
  "binding::defaultValue",
  "targetKey",
  "set target",
]);
