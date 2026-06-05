// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-for-statement-runtime-semantics-labelledevaluation
description: >
    Creation of new lexical environment for each evaluation of the statement
    body
info: |
    [...]
    11. Let bodyResult be ForBodyEvaluation(the first Expression, the second
        Expression, Statement, perIterationLets, labelSet).
    [...]

    13.7.4.8 Runtime Semantics: ForBodyEvaluation

    [...]
    3. Repeat
       [...]
       b. Let result be the result of evaluating stmt.
       [...]
       e. Perform ? CreatePerIterationEnvironment(perIterationBindings).
       [...]

    13.7.4.9 Runtime Semantics: CreatePerIterationEnvironment

    1. If perIterationBindings has any elements, then
       [...]
       e. Let thisIterationEnv be NewDeclarativeEnvironment(outer).
       f. Let thisIterationEnvRec be thisIterationEnv's EnvironmentRecord.
features: [let]
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
    console.log(error instanceof expectedCtor);
  }
};

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;


var probeFirst;
var probeSecond = null;

for (let x = 'first'; probeSecond === null; x = 'second')
  if (!probeFirst)
    probeFirst = function() { return x; };
  else
    probeSecond = function() { return x; };

assert.sameValue(probeFirst(), 'first');
assert.sameValue(probeSecond(), 'second');
