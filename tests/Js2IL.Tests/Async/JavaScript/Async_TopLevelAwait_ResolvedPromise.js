console.log("before");
const value = await Promise.resolve(42);
console.log("value:", value);
console.log("after");
