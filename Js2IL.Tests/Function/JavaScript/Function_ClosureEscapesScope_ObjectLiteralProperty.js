"use strict";

// Issue #167 repro: a capturing function escapes its scope via object literal property.

function createCalculator(factor) {
    function multiply(x) {
        return x * factor;
    }

    return {
        multiply: multiply
    };
}

const a = createCalculator(10);
console.log("a.multiply(5):", a.multiply(5));

const b = createCalculator(3);
console.log("b.multiply(7):", b.multiply(7));
