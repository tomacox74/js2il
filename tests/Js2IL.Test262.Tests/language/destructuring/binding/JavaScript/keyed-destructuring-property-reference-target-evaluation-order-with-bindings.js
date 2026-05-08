// Copyright (C) 2024 André Bargull. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-destructuring-binding-patterns-runtime-semantics-propertybindinginitialization
description: >
  Ensure correct evaluation order for binding lookups when destructuring target is var-binding.
info: |
  14.3.3.1 Runtime Semantics: PropertyBindingInitialization

    BindingProperty : PropertyName : BindingElement

    1. Let P be ? Evaluation of PropertyName.
    2. Perform ? KeyedBindingInitialization of BindingElement with arguments value, environment, and P.
    ...

  14.3.3.3 Runtime Semantics: KeyedBindingInitialization

    SingleNameBinding : BindingIdentifier Initializer_opt

    1. Let bindingId be the StringValue of BindingIdentifier.
    2. Let lhs be ? ResolveBinding(bindingId, environment).
    3. Let v be ? GetV(value, propertyName).
    4. If Initializer is present and v is undefined, then
      ...
      b. Else,
        i. Let defaultValue be ? Evaluation of Initializer.
        ii. Set v to ? GetValue(defaultValue).
    ...
    6. Return ? InitializeReferencedBinding(lhs, v).

  9.4.2 ResolveBinding ( name [ , env ] )

    ...
    4. Return ? GetIdentifierReference(env, name, strict).

  9.1.2.1 GetIdentifierReference ( env, name, strict )

    ...
    2. Let exists be ? env.HasBinding(name).
    ...

includes: [compareArray.js]
features: [Proxy]
flags: [noStrict]
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

var log = [];

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

var env = new Proxy({}, {
  has(t, pk) {
    log.push("binding::" + pk);
    return false;
  }
});

var defaultValue = 0;

var varTarget;

with (env) {
  var {
    [sourceKey]: varTarget = defaultValue
  } = source;
}

assert.compareArray(log, [
  "binding::source",
  "binding::sourceKey",
  "sourceKey",
  "binding::varTarget",
  "get source",
  "binding::defaultValue",
]);
