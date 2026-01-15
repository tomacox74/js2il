// Test async arrow function with await
const asyncArrow = async () => {
    console.log("Before await");
    const result = await Promise.resolve(42);
    console.log("After await, result:", result);
    return result;
};

asyncArrow().then((value) => {
    console.log("Final value:", value);
});

console.log("After calling async arrow");
