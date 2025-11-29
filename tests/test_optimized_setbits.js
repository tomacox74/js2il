// Test the optimized setBitsTrue with while loop

class BitArray {
    constructor(size) {
        this.wordArray = new Int32Array(1 + (size >>> 5));
    }

    setBitsTrue(range_start, step, range_stop) {
        // optimized version from the actual code
        let index = range_start;
        let wordOffset = index >>> 5;
        let wordValue = this.wordArray[wordOffset];

        while (index < range_stop) {
            const bitOffset = index & 31;
            wordValue |= (1 << bitOffset);

            index += step;
            const newwordOffset = index >>> 5;
            if (newwordOffset != wordOffset) {
                this.wordArray[wordOffset] = wordValue;
                wordOffset = newwordOffset;
                wordValue = this.wordArray[wordOffset];
            }
        }
        this.wordArray[wordOffset] = wordValue;
    }

    testBitTrue(index) {
        const wordOffset = index >>> 5;
        const bitOffset = index & 31;
        return this.wordArray[wordOffset] & (1 << bitOffset);
    }
}

function testOptimizedSetBitsTrue() {
    console.log("Testing optimized setBitsTrue:");
    
    const arr = new BitArray(50);
    
    // Mark multiples of 3
    const factor = 1;
    const step = factor * 2 + 1;  // = 3
    const start = factor * factor * 2 + factor + factor;  // = 4
    
    console.log("Marking multiples of 3: start=", start, "step=", step);
    arr.setBitsTrue(start, step, 25);
    
    console.log("\nVerification:");
    console.log("9 (idx 4) should be marked:", arr.testBitTrue(4) ? "YES" : "NO");
    console.log("11 (idx 5) should NOT be marked:", arr.testBitTrue(5) ? "NO (BUG!)" : "YES");
    console.log("15 (idx 7) should be marked:", arr.testBitTrue(7) ? "YES" : "NO");
    console.log("21 (idx 10) should be marked:", arr.testBitTrue(10) ? "YES" : "NO");
    
    // Check the != comparison
    console.log("\nTesting != comparison:");
    const a = 5;
    const b = 5;
    const c = 10;
    console.log("5 != 5:", (a != b) ? "true (BUG!)" : "false (correct)");
    console.log("5 != 10:", (a != c) ? "true (correct)" : "false (BUG!)");
}

testOptimizedSetBitsTrue();
