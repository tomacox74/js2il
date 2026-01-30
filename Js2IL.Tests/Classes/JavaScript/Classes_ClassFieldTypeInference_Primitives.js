"use strict";

class Counter {
    constructor() {
        this.value = 0;
        this.name = "test";
        this.active = true;
    }

    increment() {
        this.value = this.value + 1;
    }
}

const c = new Counter();
c.increment();
c.increment();

console.log(c.value);
console.log(c.name);
console.log(c.active);
