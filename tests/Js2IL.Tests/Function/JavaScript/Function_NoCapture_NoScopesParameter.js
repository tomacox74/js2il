"use strict";

// Non-capturing function - should NOT have scopes parameter after optimization
function add(a, b) {
    return a + b;
}

console.log(add(5, 3));
