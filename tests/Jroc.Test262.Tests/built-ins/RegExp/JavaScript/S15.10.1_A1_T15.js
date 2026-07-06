try {
    throw new Test262Error('#1.1: new RegExp("x{1,}{1}") throw SyntaxError. Actual: ' + (new RegExp("x{1,}{1}")));
} catch (e) {
  assert.sameValue(
    e instanceof SyntaxError,
    true,
    'The result of evaluating (e instanceof SyntaxError) is expected to be true'
  );
}

// TODO: Convert to assert.throws() format.
