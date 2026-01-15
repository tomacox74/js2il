// Test async function expression with await
const asyncFunc = async function() {
    console.log("Before await");
    const result = await Promise.resolve(100);
    console.log("After await, result:", result);
    return result;
};

asyncFunc().then((value) => {
    console.log("Final value:", value);
});

console.log("After calling async function expression");
