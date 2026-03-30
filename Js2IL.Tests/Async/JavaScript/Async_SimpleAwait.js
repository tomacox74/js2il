"use strict";

// Simple await test - await an already-resolved promise
async function test() {
    const result = await Promise.resolve(42);
    console.log("Result:", result);
    return result;
}

test().then((value) => {
    console.log("Final value:", value);
});
