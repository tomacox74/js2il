assert.throws(Test262Error, function() {
  BigInt({
    toString: function() {
      throw new Test262Error();
    }
  });
});
