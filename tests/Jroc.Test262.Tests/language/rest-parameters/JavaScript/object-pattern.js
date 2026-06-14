// Copyright (C) 2015 André Bargull. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-destructuring-binding-patterns
description: >
  The rest parameter can be a binding pattern.
info: |
  Destructuring Binding Patterns - Syntax

  BindingRestElement[Yield]:
    ...BindingPattern[?Yield]
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

assert.compareArray = function(actual, expected) {
  if (!Array.isArray(actual) || !Array.isArray(expected) || actual.length !== expected.length) {
    console.log(false);
    return;
  }

  for (let i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      console.log(false);
      return;
    }
  }

  console.log(true);
};

assert.throws = function(expectedCtor, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedCtor || error.constructor === expectedCtor || error.name === expectedCtor.name);
  }
};

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

function empty(...{}) {}

function emptyWithArray(...{p: []}) {}

function emptyWithObject(...{p: {}}) {}

function emptyWithLeading(x, ...{}) {}


function singleElement(...{a: b}) {}

function singleElementWithInitializer(...{a: b = 0}) {}

function singleElementWithArray(...{p: [a]}) {}

function singleElementWithObject(...{p: {a: b}}) {}

function singleElementWithLeading(x, ...{a: b}) {}


function multiElement(...{a: r, b: s, c: t}) {}

function multiElementWithInitializer(...{a: r = 0, b: s, c: t = 1}) {}

function multiElementWithArray(...{p: [a], b, q: [c]}) {}

function multiElementWithObject(...{a: {p: q}, b: {r}, c: {s = 0}}) {}

function multiElementWithLeading(x, y, ...{a: r, b: s, c: t}) {}
