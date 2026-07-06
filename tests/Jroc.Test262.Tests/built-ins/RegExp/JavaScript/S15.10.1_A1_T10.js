try {
    throw new Test262Error('#1.1: new RegExp("++a") throw SyntaxError. Actual: ' + (new RegExp("++a")));
} catch (e) {
  assert.sameValue(
    e instanceof SyntaxError,
    true,
    'The result of evaluating (e instanceof SyntaxError) is expected to be true'
  );
}

// TODO: Convert to assert.throws() format.
