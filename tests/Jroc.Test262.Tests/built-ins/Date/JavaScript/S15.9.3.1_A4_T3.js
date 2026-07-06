function PoisonedValueOf(val) {
  this.value = val;
  this.valueOf = function() {
    throw new Test262Error();
  };
  this.toString = function() {};
}

assert.throws(Test262Error, () => {
  new Date(new PoisonedValueOf(1), new PoisonedValueOf(2), new PoisonedValueOf(3), new PoisonedValueOf(4));
}, '`new Date(new PoisonedValueOf(1), new PoisonedValueOf(2), new PoisonedValueOf(3), new PoisonedValueOf(4))` throws a Test262Error exception');

assert.throws(Test262Error, () => {
  new Date(1, new PoisonedValueOf(2), new PoisonedValueOf(3), new PoisonedValueOf(4));
}, '`new Date(1, new PoisonedValueOf(2), new PoisonedValueOf(3), new PoisonedValueOf(4))` throws a Test262Error exception');

assert.throws(Test262Error, () => {
  new Date(1, 2, new PoisonedValueOf(3), new PoisonedValueOf(4));
}, '`new Date(1, 2, new PoisonedValueOf(3), new PoisonedValueOf(4))` throws a Test262Error exception');

assert.throws(Test262Error, () => {
  new Date(1, 2, 3, new PoisonedValueOf(4));
}, '`new Date(1, 2, 3, new PoisonedValueOf(4))` throws a Test262Error exception');
