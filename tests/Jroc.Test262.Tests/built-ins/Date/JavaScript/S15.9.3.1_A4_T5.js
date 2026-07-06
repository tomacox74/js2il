function PoisonedValueOf(val) {
  this.value = val;
  this.valueOf = function() {
    throw new Test262Error();
  };
  this.toString = function() {};
}

assert.throws(Test262Error, () => {
  new Date(new PoisonedValueOf(1), new PoisonedValueOf(2), new PoisonedValueOf(3), new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6));
}, '`new Date(new PoisonedValueOf(1), new PoisonedValueOf(2), new PoisonedValueOf(3), new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6))` throws a Test262Error exception');

assert.throws(Test262Error, () => {
  new Date(1, new PoisonedValueOf(2), new PoisonedValueOf(3), new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6));
}, '`new Date(1, new PoisonedValueOf(2), new PoisonedValueOf(3), new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6))` throws a Test262Error exception');

assert.throws(Test262Error, () => {
  new Date(1, 2, new PoisonedValueOf(3), new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6));
}, '`new Date(1, 2, new PoisonedValueOf(3), new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6))` throws a Test262Error exception');

assert.throws(Test262Error, () => {
  new Date(1, 2, 3, new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6));
}, '`new Date(1, 2, 3, new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6))` throws a Test262Error exception');

assert.throws(Test262Error, () => {
  new Date(1, 2, 3, 4, new PoisonedValueOf(5), new PoisonedValueOf(6));
}, '`new Date(1, 2, 3, 4, new PoisonedValueOf(5), new PoisonedValueOf(6))` throws a Test262Error exception');

assert.throws(Test262Error, () => {
  new Date(1, 2, 3, 4, 5, new PoisonedValueOf(6));
}, '`new Date(1, 2, 3, 4, 5, new PoisonedValueOf(6))` throws a Test262Error exception');
