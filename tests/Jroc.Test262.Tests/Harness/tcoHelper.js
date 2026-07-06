// Adapted from upstream test262 harness/tcoHelper.js for the shared CommonJS harness.

var $MAX_ITERATIONS = 100000;
globalThis.$MAX_ITERATIONS = $MAX_ITERATIONS;

module.exports = {
    $MAX_ITERATIONS: $MAX_ITERATIONS
};
