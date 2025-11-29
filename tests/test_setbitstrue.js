// Test setBitsTrue - the critical function in prime sieve

class BitArray {
    constructor(size) {
        this.wordArray = new Int32Array(1 + (size >>> 5));
    }

    setBitTrue(index) {
        const wordOffset = index >>> 5;
        const bitOffset = index & 31;
        this.wordArray[wordOffset] |= (1 << bitOffset);
    }

    setBitsTrue(range_start, step, range_stop) {
        // Simple version - just mark every step
        for (let index = range_start; index < range_stop; index += step) {
            this.setBitTrue(index);
        }
    }

    testBitTrue(index) {
        const wordOffset = index >>> 5;
        const bitOffset = index & 31;
        return this.wordArray[wordOffset] & (1 << bitOffset);
    }
}

function testSetBitsTrue() {
    console.log("Testing setBitsTrue with simple sieve pattern:");
    
    // Simulate marking multiples of 3
    const arr = new BitArray(50);
    
    // Mark multiples of 3 starting from 3*3=9
    // In the odd-only encoding: index for number 9 is 4 (since 2*4+1=9)
    // step = 3, start = 4
    const factor = 1;  // represents number 3 (2*1+1=3)
    const step = factor * 2 + 1;  // = 3
    const start = factor * factor * 2 + factor + factor;  // = 4
    
    console.log("Marking multiples of 3: start=", start, "step=", step);
    arr.setBitsTrue(start, step, 25);
    
    // Check results
    // Indices that should be marked: 4 (9), 7 (15), 10 (21), 13 (27), 16 (33), 19 (39), 22 (45)
    for (let i = 0; i < 25; i++) {
        const number = 2 * i + 1;
        const isMarked = arr.testBitTrue(i) ? "marked" : "prime";
        if (arr.testBitTrue(i)) {
            console.log("Index", i, "-> number", number, ":", isMarked);
        }
    }
    
    // Verify specific values
    console.log("\nVerification:");
    console.log("9 (idx 4) should be marked:", arr.testBitTrue(4) ? "YES" : "NO");
    console.log("11 (idx 5) should NOT be marked:", arr.testBitTrue(5) ? "NO (BUG!)" : "YES");
    console.log("15 (idx 7) should be marked:", arr.testBitTrue(7) ? "YES" : "NO");
}

testSetBitsTrue();
