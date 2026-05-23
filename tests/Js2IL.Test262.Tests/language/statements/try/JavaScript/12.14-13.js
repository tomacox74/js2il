// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es5id: 12.14-13
description: catch introduces scope - updates are based on scope
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

var res1 = false;
        var res2 = false;
        var res3 = false;

(function() {
        var x_12_14_13 = 'local';

            function foo() {
                this.x_12_14_13 = 'instance';
            }

            try {
                throw foo;
            }
            catch (e) {
                res1 = (x_12_14_13 === 'local');
                e();
                res2 = (x_12_14_13 === 'local');
            }
            res3 = (x_12_14_13 === 'local');
})();

assert(res1, 'res1 !== true');
assert(res2, 'res2 !== true');
assert(res3, 'res3 !== true');
