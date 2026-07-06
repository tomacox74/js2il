var obj = {};
obj[Symbol.toPrimitive] = function() {
  throw new Test262Error();
};

assert.throws(Test262Error, function() {
  isNaN(obj);
});
