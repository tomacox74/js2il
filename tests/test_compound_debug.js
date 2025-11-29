// Debug version with more console output

class TestClass {
    constructor() {
        this.arr = new Int32Array(5);
    }

    test() {
        let wordOffset = 2;
        const mask = 8;
        
        console.log("wordOffset =", wordOffset);
        console.log("mask =", mask);
        console.log("Before: this.arr[wordOffset] =", this.arr[wordOffset]);
        
        // Manual breakdown of the operation
        const currentValue = this.arr[wordOffset];
        console.log("currentValue =", currentValue);
        const newValue = currentValue | mask;
        console.log("newValue = currentValue | mask =", newValue);
        
        this.arr[wordOffset] |= mask;
        console.log("After |=: this.arr[wordOffset] =", this.arr[wordOffset]);
    }
}

new TestClass().test();
