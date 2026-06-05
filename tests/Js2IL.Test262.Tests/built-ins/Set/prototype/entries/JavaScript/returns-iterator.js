// Copyright (C) 2014 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-set.prototype.entries
description: >
    Set.prototype.entries ( )

    ...
    2. Return CreateSetIterator(S, "key+value").


    23.2.5.1 CreateSetIterator Abstract Operation

    ...
    7. Return iterator.


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

assert.throws = function(expectedCtor, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedCtor);
  }
};

function compareArray(actual, expected) {
  if (!Array.isArray(actual) || !Array.isArray(expected) || actual.length !== expected.length) {
    return false;
  }

  for (var i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      return false;
    }
  }

  return true;
}

assert.compareArray = function(actual, expected) {
  console.log(compareArray(actual, expected));
};

function verifyProperty(obj, name, desc) {
  var originalDesc = Object.getOwnPropertyDescriptor(obj, name);
  console.log(!!originalDesc);
  if ('value' in desc) {
    console.log(Object.is(originalDesc.value, desc.value));
  }
  if ('writable' in desc) {
    console.log(originalDesc.writable === desc.writable);
  }
  if ('enumerable' in desc) {
    console.log(originalDesc.enumerable === desc.enumerable);
  }
  if ('configurable' in desc) {
    console.log(originalDesc.configurable === desc.configurable);
  }
  if ('get' in desc) {
    console.log(Object.is(originalDesc.get, desc.get));
  }
  if ('set' in desc) {
    console.log(Object.is(originalDesc.set, desc.set));
  }
}

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;


var set = new Set();
set.add(1);
set.add(2);
set.add(3);

var iterator = set.entries();
var result;

result = iterator.next();
assert.sameValue(result.value[0], 1, 'First result `value` ("key")');
assert.sameValue(result.value[1], 1, 'First result `value` ("value")');
assert.sameValue(result.value.length, 2, 'First result `value` (length)');
assert.sameValue(result.done, false, 'First result `done` flag');

result = iterator.next();
assert.sameValue(result.value[0], 2, 'Second result `value` ("key")');
assert.sameValue(result.value[1], 2, 'Second result `value` ("value")');
assert.sameValue(result.value.length, 2, 'Second result `value` (length)');
assert.sameValue(result.done, false, 'Second result `done` flag');

result = iterator.next();
assert.sameValue(result.value[0], 3, 'Third result `value` ("key")');
assert.sameValue(result.value[1], 3, 'Third result `value` ("value")');
assert.sameValue(result.value.length, 2, 'Third result `value` (length)');
assert.sameValue(result.done, false, 'Third result `done` flag');

result = iterator.next();
assert.sameValue(result.value, undefined, 'Exhausted result `value`');
assert.sameValue(result.done, true, 'Exhausted result `done` flag');

result = iterator.next();
assert.sameValue(
  result.value, undefined, 'Exhausted result `value` (repeated request)'
);
assert.sameValue(
  result.done, true, 'Exhausted result `done` flag (repeated request)'
);
