// Simplified BitArray test mimicking the prime sieve

class BitArray {
    constructor(size) {
        this.wordArray = new Int32Array(1 + (size >>> 5));
        console.log("Created BitArray with size", size, "->", this.wordArray.length, "words");
    }

    setBitTrue(index) {
        const wordOffset = index >>> 5;
        const bitOffset = index & 31;
        this.wordArray[wordOffset] |= (1 << bitOffset);
    }

    testBitTrue(index) {
        const wordOffset = index >>> 5;
        const bitOffset = index & 31;
        return this.wordArray[wordOffset] & (1 << bitOffset);
    }

    searchBitFalse(index) {
        while (this.testBitTrue(index)) { 
            index++; 
        }
        return index;
    }
}

function testBitArray() {
    const arr = new BitArray(100);
    
    // Set some bits
    arr.setBitTrue(0);
    arr.setBitTrue(5);
    arr.setBitTrue(10);
    
    console.log("Bit 0 set?", arr.testBitTrue(0) ? "yes" : "no", "(expected: yes)");
    console.log("Bit 1 set?", arr.testBitTrue(1) ? "yes" : "no", "(expected: no)");
    console.log("Bit 5 set?", arr.testBitTrue(5) ? "yes" : "no", "(expected: yes)");
    console.log("Bit 10 set?", arr.testBitTrue(10) ? "yes" : "no", "(expected: yes)");
    
    // Test searchBitFalse
    const found = arr.searchBitFalse(0);
    console.log("First false bit from 0:", found, "(expected: 1)");
    
    const found2 = arr.searchBitFalse(5);
    console.log("First false bit from 5:", found2, "(expected: 6)");
}

testBitArray();
