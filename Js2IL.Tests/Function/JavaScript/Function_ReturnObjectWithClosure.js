"use strict";\r\n\r\n// Test: Function returns object literal containing inner function that captures outer variable

function createCalculator(factor) {
    // Inner function captures 'factor' from outer function scope
    function multiply(x) {
        return x * factor;
    }
    
    // Return object literal with the inner function
    return {
        multiply: multiply,
        factor: factor
    };
}

// Invoke the outer function to get the object
const calc = createCalculator(10);

// Call the method on the returned object
console.log("multiply(5):", calc.multiply(5));
console.log("factor:", calc.factor);

// Create another with different captured value
const calc2 = createCalculator(3);
console.log("calc2.multiply(7):", calc2.multiply(7));
