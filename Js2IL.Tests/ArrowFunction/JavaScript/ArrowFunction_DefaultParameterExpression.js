"use strict";\r\n\r\n// Test default parameter expressions that reference other parameters in arrow functions
const calculate = (a, b = a * 2, c = a + b) => {
    console.log(c);
};

// Test parameter referencing other parameters
calculate(5);
calculate(5, 8);
calculate(5, 8, 20);
