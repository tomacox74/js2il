// Test awaiting a pending promise that resolves asynchronously
console.log("Before async function");

async function test() {
    console.log("Before await");
    
    const result = await new Promise((resolve) => {
        setTimeout(() => {
            console.log("Timer fired - resolving");
            resolve(42);
        }, 10);
    });
    
    console.log("After await, result:", result);
    return result;
}

test().then((value) => {
    console.log("Final value:", value);
});

console.log("After calling async function");
