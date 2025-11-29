// Test with literal vs variable index

class TestClass {
    constructor() {
        this.arr = new Int32Array(5);
    }

    test() {
        console.log("Test 1: Literal index");
        this.arr[2] |= 8;
        console.log("arr[2] =", this.arr[2], "(expected: 8)");
        
        console.log("\nTest 2: Const variable index");
        const idx = 3;
        this.arr[idx] |= 16;
        console.log("arr[3] =", this.arr[3], "(expected: 16)");
        
        console.log("\nTest 3: Let variable index");
        let wordOffset = 4;
        this.arr[wordOffset] |= 32;
        console.log("arr[4] =", this.arr[4], "(expected: 32)");
    }
}

new TestClass().test();
