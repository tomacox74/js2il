function delay(value) {
    return new Promise((resolve) => setTimeout(() => resolve(value), 1));
}

console.log("before");
const value = await delay("done");
console.log("value:", value);
console.log("after");
