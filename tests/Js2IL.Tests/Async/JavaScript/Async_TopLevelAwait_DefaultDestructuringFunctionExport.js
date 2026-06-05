function pick({ value = "default" } = {}) {
    return value;
}

await Promise.resolve();

console.log(pick());
console.log(pick({ value: "provided" }));
