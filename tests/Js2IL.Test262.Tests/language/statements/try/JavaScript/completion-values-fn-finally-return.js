// Copyright (C) 2020 Salesforce.com. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-try-statement-runtime-semantics-evaluation
description: >
  Returns the correct completion values of try-catch-finally(Return) in functions
info: |
  TryStatement : try Block Catch Finally

    Let B be the result of evaluating Block.
    If B.[[Type]] is throw, let C be CatchClauseEvaluation of Catch with argument B.[[Value]].
    Else, let C be B.
    Let F be the result of evaluating Finally.
    If F.[[Type]] is normal, set F to C.
    Return Completion(UpdateEmpty(F, undefined)).
---*/

// 1: try Return, catch Return, finally Return; Completion: finally
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

var count = {
  catch: 0,
  finally: 0
};

var fn = function() {
  try {
    return 'try';
  } catch(e) {
    count.catch += 1;
    return 'catch';
  } finally {
    count.finally += 1;
    return 'finally';
  }
  return 'wat';
};

assert.sameValue(fn(), 'finally', '1: try Return, catch Return, finally Return; Completion: finally');
assert.sameValue(count.catch, 0, '1');
assert.sameValue(count.finally, 1, '1');

// 2: try Abrupt, catch Return, finally Return; Completion: finally
count.catch = 0;
count.finally = 0;
fn = function() {
  try {
    throw 'try';
  } catch(e) {
    count.catch += 1;
    return 'catch';
  } finally {
    count.finally += 1;
    return 'finally';
  }
  return 'wat';
};

assert.sameValue(fn(), 'finally', '2: try Abrupt, catch Return, finally Return; Completion: finally');
assert.sameValue(count.catch, 1, '2: catch count');
assert.sameValue(count.finally, 1, '2: fiinally count');

// 3: try Abrupt, catch Abrupt, finally Normal; Completion: finally
count.catch = 0;
count.finally = 0;
fn = function() {
  try {
    throw 'try';
  } catch(e) {
    count.catch += 1;
    throw 'catch';
  } finally {
    count.finally += 1;
    return 'finally';
  }
  return 'wat';
};

assert.sameValue(fn(), 'finally', fn, '3: try Abrupt, catch Abrupt, finally Normal; Completion: finally');
assert.sameValue(count.catch, 1, '3: catch count');
assert.sameValue(count.finally, 1, '3: fiinally count');
