// Copyright (C) 2021 Alexey Shvayka. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-destructuring-binding-patterns-runtime-semantics-restbindinginitialization
description: >
  Proxy's "get" trap is not invoked for non-enumerable keys.
info: |
  BindingRestProperty : ... BindingIdentifier

  [...]
  3. Perform ? CopyDataProperties(restObj, value, excludedNames).

  CopyDataProperties ( target, source, excludedItems )

  [...]
  5. Let keys be ? from.[[OwnPropertyKeys]]().
  6. For each element nextKey of keys in List order, do
    [...]
    c. If excluded is false, then
      i. Let desc be ? from.[[GetOwnProperty]](nextKey).
      ii. If desc is not undefined and desc.[[Enumerable]] is true, then
        1. Let propValue be ? Get(from, nextKey).
        2. Perform ! CreateDataPropertyOrThrow(target, nextKey, propValue).

  [[OwnPropertyKeys]] ( )

  [...]
  7. Let trapResultArray be ? Call(trap, handler, « target »).
  8. Let trapResult be ? CreateListFromArrayLike(trapResultArray, « String, Symbol »).
  [...]
  23. Return trapResult.
features: [object-rest, destructuring-binding, Proxy, Symbol]
includes: [compareArray.js, propertyHelper.js]
---*/

function assert(value) {
  console.log(!!value);
}

assert.sameValue = function(actual, expected) {
  console.log(Object.is(actual, expected));
};

assert.notSameValue = function(actual, unexpected) {
  console.log(!Object.is(actual, unexpected));
};

function compareArray(actual, expected) {
  if (!Array.isArray(actual) || !Array.isArray(expected) || actual.length !== expected.length) {
    return false;
  }

  for (let i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      return false;
    }
  }

  return true;
}

assert.compareArray = function(actual, expected) {
  console.log(compareArray(actual, expected));
};

assert.throws = function(expectedCtor, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedCtor);
  }
};

function verifyProperty(obj, name, desc) {
  var actual = Object.getOwnPropertyDescriptor(obj, name);
  if (actual === undefined) {
    console.log(false);
    return;
  }

  var ok = true;

  if (Object.prototype.hasOwnProperty.call(desc, 'value')) {
    ok = ok && Object.is(actual.value, desc.value);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'writable')) {
    ok = ok && Object.is(actual.writable, desc.writable);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'enumerable')) {
    ok = ok && Object.is(actual.enumerable, desc.enumerable);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'configurable')) {
    ok = ok && Object.is(actual.configurable, desc.configurable);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'get')) {
    ok = ok && Object.is(actual.get, desc.get);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'set')) {
    ok = ok && Object.is(actual.set, desc.set);
  }

  console.log(ok);
}

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

var VALUE_GOPD = "VALUE_GOPD";
var VALUE_GET = "VALUE_GET";

var dontEnumSymbol = Symbol("dont_enum_symbol");
var enumerableSymbol = Symbol("enumerable_symbol");

var dontEnumKeys = [dontEnumSymbol, "dontEnumString", "0"];
var enumerableKeys = [enumerableSymbol, "enumerableString", "1"];
var ownKeysResult = [...dontEnumKeys, ...enumerableKeys];

var getOwnKeys = [];
var getKeys = [];
var proxy = new Proxy({}, {
  getOwnPropertyDescriptor: function(_target, key) {
    getOwnKeys.push(key);
    var isEnumerable = enumerableKeys.indexOf(key) !== -1;
    return {value: VALUE_GOPD, writable: false, enumerable: isEnumerable, configurable: true};
  },
  get: function(_target, key) {
    getKeys.push(key);
    return VALUE_GET;
  },
  ownKeys: function() {
    return ownKeysResult;
  },
});

var {...rest} = proxy;
assert.compareArray(getOwnKeys, ownKeysResult);
assert.compareArray(getKeys, enumerableKeys);

verifyProperty(rest, enumerableSymbol, {value: VALUE_GET, writable: true, enumerable: true, configurable: true});
verifyProperty(rest, "enumerableString", {value: VALUE_GET, writable: true, enumerable: true, configurable: true});
verifyProperty(rest, "1", {value: VALUE_GET, writable: true, enumerable: true, configurable: true});
