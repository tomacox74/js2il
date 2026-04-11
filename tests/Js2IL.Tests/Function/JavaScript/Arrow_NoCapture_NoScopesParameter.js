"use strict";

// Non-capturing arrow function - should NOT have scopes parameter after optimization
const multiply = (x, y) => x * y;

console.log(multiply(4, 7));
