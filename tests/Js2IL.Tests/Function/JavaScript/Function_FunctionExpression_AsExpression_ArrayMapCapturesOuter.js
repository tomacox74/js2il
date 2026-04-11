"use strict";

// Test: FunctionExpression as an expression value (PL3.6)
// Uses Array.map to invoke the function value, so the IR pipeline must:
// 1) create a delegate for the FunctionExpression
// 2) bind the correct scopes array for captured variables

function makeOffsetMapper(offset) {
    return [1, 2, 3].map(function (n) {
        return n + offset;
    }).join(",");
}

console.log(makeOffsetMapper(10));
