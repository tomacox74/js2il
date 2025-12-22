function logNameAge(name, age) {
    console.log(name + " " + age);
}

setImmediate(logNameAge, "Alice", 42);
console.log("setImmediate with args scheduled.");
