// Minimal reproduction of compound assignment bug with local variable index

class TestClass {
    constructor() {
        this.arr = new Int32Array(5);
    }

    test() {
        let wordOffset = 2;
        const mask = 8;
        
        console.log("Before: this.arr[2] =", this.arr[2]);
        this.arr[wordOffset] |= mask;
        console.log("After this.arr[wordOffset] |= 8: this.arr[2] =", this.arr[2], "(expected: 8)");
    }
}

new TestClass().test();
