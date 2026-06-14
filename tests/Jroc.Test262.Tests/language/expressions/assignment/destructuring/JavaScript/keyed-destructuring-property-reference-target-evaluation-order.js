// Copyright (C) 2017 André Bargull. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-runtime-semantics-keyeddestructuringassignmentevaluation
description: >
    Ensure correct evaluation order when destructuring target is property reference.
info: |
    12.15.5.2 Runtime Semantics: DestructuringAssignmentEvaluation

    AssignmentProperty : PropertyName : AssignmentElement

    1. Let name be the result of evaluating PropertyName.
    2. ReturnIfAbrupt(name).
    3. Return the result of performing KeyedDestructuringAssignmentEvaluation of
       AssignmentElement with value and name as the arguments. 

    12.15.5.4 Runtime Semantics: KeyedDestructuringAssignmentEvaluation

    1. If DestructuringAssignmentTarget is neither an ObjectLiteral nor an ArrayLiteral, then
        a. Let lref be the result of evaluating DestructuringAssignmentTarget.
        b. ReturnIfAbrupt(lref).
    2. Let v be ? GetV(value, propertyName).
    ...
    4. Else, let rhsValue be v.
    ...
    7. Return ? PutValue(lref, rhsValue).
includes: [compareArray.js]
---*/

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message === undefined ? '' : String(message);
}
function __test262SameValue(a, b) {
  return Object.is(a, b);
}
function compareArray(actual, expected) {
  if (!actual || !expected || actual.length !== expected.length) {
    return false;
  }
  for (let i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      return false;
    }
  }
  return true;
}
function verifyProperty(obj, name, desc) {
  const actual = Object.getOwnPropertyDescriptor(obj, name);
  let ok = !!actual;
  if ('value' in desc) ok = ok && Object.is(actual.value, desc.value);
  if ('writable' in desc) ok = ok && actual.writable === desc.writable;
  if ('enumerable' in desc) ok = ok && actual.enumerable === desc.enumerable;
  if ('configurable' in desc) ok = ok && actual.configurable === desc.configurable;
  if ('get' in desc) ok = ok && actual.get === desc.get;
  if ('set' in desc) ok = ok && actual.set === desc.set;
  console.log(ok);
  return ok;
}
var assert = function assert(condition) {
  console.log(!!condition);
};
assert.sameValue = function(actual, expected) {
  console.log(__test262SameValue(actual, expected));
};
assert.notSameValue = function(actual, unexpected) {
  console.log(!__test262SameValue(actual, unexpected));
};
assert.compareArray = function(actual, expected) {
  console.log(compareArray(actual, expected));
};
assert.throws = function(ExpectedError, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof ExpectedError || error.constructor === ExpectedError || error.name === ExpectedError.name);
  }
};

var log = [];

function source() {
    log.push("source");
    return {
        get p() {
            log.push("get");
        }
    };
}
function target() {
    log.push("target");
    return {
        set q(v) {
            log.push("set");
        }
    };
}
function sourceKey() {
    log.push("source-key");
    return {
        toString: function() {
            log.push("source-key-tostring");
            return "p";
        }
    };
}
function targetKey() {
    log.push("target-key");
    return {
        toString: function() {
            log.push("target-key-tostring");
            return "q";
        }
    };
}

({[sourceKey()]: target()[targetKey()]} = source());

assert.compareArray(log, [
    "source", "source-key", "source-key-tostring",
    "target", "target-key",
    "get", "target-key-tostring", "set",
]);

