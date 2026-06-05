// This file was procedurally generated from the following sources:
// - src/async-generators/yield-star-async-next.case
// - src/async-generators/default/async-expression.template
/*---
description: Execution order for yield* with async iterator and next() (Unnamed async generator expression)
esid: prod-AsyncGeneratorExpression
features: [Symbol.iterator, async-iteration, Symbol.asyncIterator]
flags: [generated, async]
info: |
    Async Generator Function Definitions

    AsyncGeneratorExpression :
      async [no LineTerminator here] function * BindingIdentifier ( FormalParameters ) {
        AsyncGeneratorBody }


    
---*/

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

function $ERROR(message) {
  throw new Test262Error(message);
}

function $DONE(error) {
  if (error) {
    throw error;
  }
}

function __sameValue(actual, expected) {
  return Object.is(actual, expected);
}

function __assertResult(passed, message) {
  console.log(!!passed);
  if (!passed) {
    throw new Error(message || 'Assertion failed');
  }
}

function assert(condition, message) {
  __assertResult(!!condition, message);
}

assert.sameValue = function(actual, expected, message) {
  __assertResult(__sameValue(actual, expected), message || 'Expected SameValue');
};

assert.notSameValue = function(actual, unexpected, message) {
  __assertResult(!__sameValue(actual, unexpected), message || 'Expected values to differ');
};

assert.throws = function(expectedErrorConstructor, fn, message) {
  var passed = false;
  try {
    fn();
  } catch (error) {
    passed = error instanceof expectedErrorConstructor ||
      (error && error.constructor === expectedErrorConstructor) ||
      (error && expectedErrorConstructor && error.name === expectedErrorConstructor.name);
  }
  __assertResult(passed, message || 'Expected function to throw');
};

assert.compareArray = function(actual, expected, message) {
  var passed = Array.isArray(actual) && Array.isArray(expected) && actual.length === expected.length;
  if (passed) {
    for (var i = 0; i < actual.length; i++) {
      if (!__sameValue(actual[i], expected[i])) {
        passed = false;
        break;
      }
    }
  }
  __assertResult(passed, message || 'Expected arrays to compare equal');
};

function verifyProperty(object, name, desc) {
  var actual = Object.getOwnPropertyDescriptor(object, name);
  var passed = !!actual;

  if (passed && Object.prototype.hasOwnProperty.call(desc, 'value')) {
    passed = __sameValue(actual.value, desc.value);
  }
  if (passed && Object.prototype.hasOwnProperty.call(desc, 'writable')) {
    passed = actual.writable === desc.writable;
  }
  if (passed && Object.prototype.hasOwnProperty.call(desc, 'enumerable')) {
    passed = actual.enumerable === desc.enumerable;
  }
  if (passed && Object.prototype.hasOwnProperty.call(desc, 'configurable')) {
    passed = actual.configurable === desc.configurable;
  }
  if (passed && Object.prototype.hasOwnProperty.call(desc, 'get')) {
    passed = actual.get === desc.get;
  }
  if (passed && Object.prototype.hasOwnProperty.call(desc, 'set')) {
    passed = actual.set === desc.set;
  }

  __assertResult(passed, 'verifyProperty failed for ' + name);
}

var log = [];
var obj = {
  get [Symbol.iterator]() {
    log.push({ name: "get [Symbol.iterator]" });
  },
  get [Symbol.asyncIterator]() {
    log.push({
      name: "get [Symbol.asyncIterator]",
      thisValue: this
    });
    return function() {
      log.push({
        name: "call [Symbol.asyncIterator]",
        thisValue: this,
        args: [...arguments]
      });
      var nextCount = 0;
      return {
        name: "asyncIterator",
        get next() {
          log.push({
            name: "get next",
            thisValue: this
          });
          return function() {
            log.push({
              name: "call next",
              thisValue: this,
              args: [...arguments]
            });

            nextCount++;
            if (nextCount == 1) {
              return {
                name: "next-promise-1",
                get then() {
                  log.push({
                    name: "get next then (1)",
                    thisValue: this
                  });
                  return function(resolve) {
                    log.push({
                      name: "call next then (1)",
                      thisValue: this,
                      args: [...arguments]
                    });

                    resolve({
                      name: "next-result-1",
                      get value() {
                        log.push({
                          name: "get next value (1)",
                          thisValue: this
                        });
                        return "next-value-1";
                      },
                      get done() {
                        log.push({
                          name: "get next done (1)",
                          thisValue: this
                        });
                        return false;
                      }
                    });
                  };
                }
              };
            }

            return {
              name: "next-promise-2",
              get then() {
                log.push({
                  name: "get next then (2)",
                  thisValue: this
                });
                return function(resolve) {
                  log.push({
                    name: "call next then (2)",
                    thisValue: this,
                    args: [...arguments]
                  });

                  resolve({
                    name: "next-result-2",
                    get value() {
                      log.push({
                        name: "get next value (2)",
                        thisValue: this
                      });
                      return "next-value-2";
                    },
                    get done() {
                      log.push({
                        name: "get next done (2)",
                        thisValue: this
                      });
                      return true;
                    }
                  });
                };
              }
            };
          };
        }
      };
    };
  }
};



var callCount = 0;

var gen = async function *() {
  callCount += 1;
  log.push({ name: "before yield*" });
    var v = yield* obj;
    log.push({
      name: "after yield*",
      value: v
    });
    return "return-value";

};

var iter = gen();

assert.sameValue(log.length, 0, "log.length");

iter.next("next-arg-1").then(v => {
  assert.sameValue(log[0].name, "before yield*");

  assert.sameValue(log[1].name, "get [Symbol.asyncIterator]");
  assert.sameValue(log[1].thisValue, obj, "get [Symbol.asyncIterator] thisValue");

  assert.sameValue(log[2].name, "call [Symbol.asyncIterator]");
  assert.sameValue(log[2].thisValue, obj, "[Symbol.asyncIterator] thisValue");
  assert.sameValue(log[2].args.length, 0, "[Symbol.asyncIterator] args.length");

  assert.sameValue(log[3].name, "get next");
  assert.sameValue(log[3].thisValue.name, "asyncIterator", "get next thisValue");

  assert.sameValue(log[4].name, "call next");
  assert.sameValue(log[4].thisValue.name, "asyncIterator", "next thisValue");
  assert.sameValue(log[4].args.length, 1, "next args.length");
  assert.sameValue(log[4].args[0], undefined, "next args[0]");

  assert.sameValue(log[5].name, "get next then (1)");
  assert.sameValue(log[5].thisValue.name, "next-promise-1", "get next then thisValue");

  assert.sameValue(log[6].name, "call next then (1)");
  assert.sameValue(log[6].thisValue.name, "next-promise-1", "next then thisValue");
  assert.sameValue(log[6].args.length, 2, "next then args.length");
  assert.sameValue(typeof log[6].args[0], "function", "next then args[0]");
  assert.sameValue(typeof log[6].args[1], "function", "next then args[1]");

  assert.sameValue(log[7].name, "get next done (1)");
  assert.sameValue(log[7].thisValue.name, "next-result-1", "get next done thisValue");

  assert.sameValue(log[8].name, "get next value (1)");
  assert.sameValue(log[8].thisValue.name, "next-result-1", "get next value thisValue");

  assert.sameValue(v.value, "next-value-1");
  assert.sameValue(v.done, false);

  assert.sameValue(log.length, 9, "log.length");

  iter.next("next-arg-2").then(v => {
    assert.sameValue(log[9].name, "call next");
    assert.sameValue(log[9].thisValue.name, "asyncIterator", "next thisValue");
    assert.sameValue(log[9].args.length, 1, "next args.length");
    assert.sameValue(log[9].args[0], "next-arg-2", "next args[0]");

    assert.sameValue(log[10].name, "get next then (2)");
    assert.sameValue(log[10].thisValue.name, "next-promise-2", "get next then thisValue");

    assert.sameValue(log[11].name, "call next then (2)");
    assert.sameValue(log[11].thisValue.name, "next-promise-2", "next then thisValue");
    assert.sameValue(log[11].args.length, 2, "next then args.length");
    assert.sameValue(typeof log[11].args[0], "function", "next then args[0]");
    assert.sameValue(typeof log[11].args[1], "function", "next then args[1]");

    assert.sameValue(log[12].name, "get next done (2)");
    assert.sameValue(log[12].thisValue.name, "next-result-2", "get next done thisValue");

    assert.sameValue(log[13].name, "get next value (2)");
    assert.sameValue(log[13].thisValue.name, "next-result-2", "get next value thisValue");

    assert.sameValue(log[14].name, "after yield*");
    assert.sameValue(log[14].value, "next-value-2");

    assert.sameValue(v.value, "return-value");
    assert.sameValue(v.done, true);

    assert.sameValue(log.length, 15, "log.length");
  }).then($DONE, $DONE);
}).catch($DONE);

assert.sameValue(callCount, 1);
