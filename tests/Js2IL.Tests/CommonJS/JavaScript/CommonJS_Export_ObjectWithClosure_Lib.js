"use strict";

// Issue #167 repro lib: exported functions capture variables and escape their defining scope.

const moduleFactor = 7;

function multiplyModuleFactor(x) {
    return x * moduleFactor;
}

function createCalculator(factor) {
    function multiply(x) {
        return x * factor;
    }

    return {
        multiply: multiply,
        factor: factor
    };
}

module.exports = {
    multiplyModuleFactor: multiplyModuleFactor,
    createCalculator: createCalculator
};
