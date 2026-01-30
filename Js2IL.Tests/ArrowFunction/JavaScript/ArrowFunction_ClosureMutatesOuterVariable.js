"use strict";\r\n\r\n// Test: Arrow function modifies captured variable from outer function scope
// This tests the IR pipeline's ability to emit stfld for captured variable writes with arrow functions

function createCounter(start) {
    let count = start;
    
    const increment = () => {
        count = count + 1;
        return count;
    };
    
    return increment;
}

const counter = createCounter(10);
console.log("First increment:", counter());
console.log("Second increment:", counter());
console.log("Third increment:", counter());
