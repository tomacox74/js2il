// Minimal test - just like test_compound_variations but accessing another variable

class TestClass {
    constructor() {
        this.arr = new Int32Array(5);
    }

    test() {
        let wordOffset = 2;
        const mask = 8;
        
        console.log("Before: arr[2] =", this.arr[2]);
        this.arr[wordOffset] |= mask;  // Using BOTH variables
        console.log("After: arr[2] =", this.arr[2], "(expected: 8)");
    }
}

new TestClass().test();
