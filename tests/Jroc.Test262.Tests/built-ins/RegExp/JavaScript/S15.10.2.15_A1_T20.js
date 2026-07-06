try {
  throw new Test262Error('#1.1: /[\\u0061d-G]/.exec("a") throw SyntaxError. Actual: ' + (new RegExp("[\\u0061d-G]").exec("a")));
} catch (e) {
  assert.sameValue(
    e instanceof SyntaxError,
    true,
    'The result of evaluating (e instanceof SyntaxError) is expected to be true'
  );
}

// TODO: Convert to assert.throws() format.
