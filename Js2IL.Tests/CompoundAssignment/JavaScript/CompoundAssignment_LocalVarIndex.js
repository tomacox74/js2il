"use strict";

// Test compound bitwise OR with local variable index
// This reproduces a bug where using a local variable as array index in compound assignment produces incorrect results

class TestClass {
    constructor() {
        this.arr = new Int32Array(5);
    }

    test() {
        let wordOffset = 2;
        const mask = 8;
        
        console.log(this.arr[wordOffset]);
        this.arr[wordOffset] |= mask;
        console.log(this.arr[wordOffset]);
    }
}

new TestClass().test();
