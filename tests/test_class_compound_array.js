// Test compound assignment on array inside a class method

class TestClass {
    constructor() {
        this.wordArray = new Int32Array(10);
    }

    testMethod() {
        console.log("Inside testMethod:");
        console.log("  Initial this.wordArray[0]:", this.wordArray[0]);
        
        // Simple assignment
        this.wordArray[0] = 5;
        console.log("  After this.wordArray[0] = 5:", this.wordArray[0]);
        
        // Compound OR assignment
        this.wordArray[0] |= 2;
        console.log("  After this.wordArray[0] |= 2:", this.wordArray[0], "(expected: 7)");
        
        // With variable index
        const idx = 1;
        this.wordArray[idx] = 10;
        console.log("  After this.wordArray[1] = 10:", this.wordArray[1]);
        
        this.wordArray[idx] |= 4;
        console.log("  After this.wordArray[1] |= 4:", this.wordArray[1], "(expected: 14)");
        
        // More complex expression for index
        const wordOffset = 2;
        const mask = 8;
        this.wordArray[wordOffset] |= mask;
        console.log("  After this.wordArray[2] |= 8:", this.wordArray[2], "(expected: 8)");
        
        this.wordArray[wordOffset] |= 16;
        console.log("  After this.wordArray[2] |= 16:", this.wordArray[2], "(expected: 24)");
    }
}

const obj = new TestClass();
obj.testMethod();
