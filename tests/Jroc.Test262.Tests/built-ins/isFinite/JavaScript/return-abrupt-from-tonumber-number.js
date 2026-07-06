var obj1 = {
  valueOf: function() {
    throw new Test262Error();
  }
};

var obj2 = {
  toString: function() {
    throw new Test262Error();
  }
};

assert.throws(Test262Error, function() {
  isFinite(obj1);
}, "valueOf");

assert.throws(Test262Error, function() {
  isFinite(obj2);
}, "toString");
