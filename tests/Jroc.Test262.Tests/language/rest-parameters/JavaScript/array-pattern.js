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

function empty(...[]) {}

function emptyWithArray(...[[]]) {}

function emptyWithObject(...[{}]) {}

function emptyWithRest(...[...[]]) {}

function emptyWithLeading(x, ...[]) {}


function singleElement(...[a]) {}

function singleElementWithInitializer(...[a = 0]) {}

function singleElementWithArray(...[[a]]) {}

function singleElementWithObject(...[{p: q}]) {}

function singleElementWithRest(...[...a]) {}

function singleElementWithLeading(x, ...[a]) {}


function multiElement(...[a, b, c]) {}

function multiElementWithInitializer(...[a = 0, b, c = 1]) {}

function multiElementWithArray(...[[a], b, [c]]) {}

function multiElementWithObject(...[{p: q}, {r}, {s = 0}]) {}

function multiElementWithRest(...[a, b, ...c]) {}

function multiElementWithLeading(x, y, ...[a, b, c]) {}
