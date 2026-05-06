// Copyright (C) 2014 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 14.5
description: >
    class arguments access
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

var constructCounts = {
  base: 0,
  subclass: 0,
  subclass2: 0
};

class Base {
  constructor() {
    constructCounts.base++;
    assert.sameValue(arguments.length, 2, "The value of `arguments.length` is `2`");
    assert.sameValue(arguments[0], 1, "The value of `arguments[0]` is `1`");
    assert.sameValue(arguments[1], 2, "The value of `arguments[1]` is `2`");
  }
}

var b = new Base(1, 2);

class Subclass extends Base {
  constructor() {
    constructCounts.subclass++;
    assert.sameValue(arguments.length, 2, "The value of `arguments.length` is `2`");
    assert.sameValue(arguments[0], 3, "The value of `arguments[0]` is `3`");
    assert.sameValue(arguments[1], 4, "The value of `arguments[1]` is `4`");
    super(1, 2);
  }
}

var s = new Subclass(3, 4);
assert.sameValue(Subclass.length, 0, "The value of `Subclass.length` is `0`, because there are 0 formal parameters");

class Subclass2 extends Base {
  constructor(x, y) {
    constructCounts.subclass2++;
    assert.sameValue(arguments.length, 2, "The value of `arguments.length` is `2`");
    assert.sameValue(arguments[0], 3, "The value of `arguments[0]` is `3`");
    assert.sameValue(arguments[1], 4, "The value of `arguments[1]` is `4`");
    super(1, 2);
  }
}

var s2 = new Subclass2(3, 4);
assert.sameValue(Subclass2.length, 2, "The value of `Subclass2.length` is `2`, because there are 2 formal parameters");


assert.sameValue(constructCounts.base, 3, "The value of `constructCounts.base` is `3`");
assert.sameValue(constructCounts.subclass, 1, "The value of `constructCounts.subclass` is `1`");
assert.sameValue(constructCounts.subclass2, 1, "The value of `constructCounts.subclass2` is `1`");
