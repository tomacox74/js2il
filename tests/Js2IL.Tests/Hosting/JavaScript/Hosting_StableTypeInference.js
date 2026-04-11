"use strict";

// A function where the return variable is set inside a loop.
// The AST-only heuristic in the contract emitter would see `return sum` with
// a local (non-top-level) identifier and fall back to `object`.
// Stable type inference detects that `sum` is always a double and propagates
// that through `Scope.StableReturnClrType`, improving the contract signature.
function calculateSum(n) {
    let sum = 0;
    for (let i = 1; i <= n; i++) {
        sum = sum + i;
    }
    return sum;
}

module.exports = {
    calculateSum,
};
