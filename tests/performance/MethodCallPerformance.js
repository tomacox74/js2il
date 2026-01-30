"use strict";

// Performance test: Class method calling another method repeatedly
class Counter {
    constructor() {
        this.value = 0;
    }

    increment() {
        this.value = this.value + 1;
    }

    runTest(iterations) {
        for (let i = 0; i < iterations; i++) {
            this.increment();
        }
    }
}

const startTime = Date.now();
const counter = new Counter();
counter.runTest(100000000); // 100 million iterations
const endTime = Date.now();

console.log(`Completed ${counter.value} iterations`);
console.log(`Time: ${endTime - startTime}ms`);
