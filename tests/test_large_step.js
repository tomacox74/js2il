// Test the large-step path in setBitsTrue

const WORD_SIZE = 32;

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
        console.log("setBitsTrue called: start=", range_start, "step=", step, "stop=", range_stop);
        
        if (step > WORD_SIZE/2) { 
            console.log("  Using LARGE STEP path (step > 16)");
            let range_stop_unique =  range_start + 32 * step;
            if (range_stop_unique > range_stop) {
                console.log("    Using simple loop (range not large enough)");
                for (let index = range_start; index < range_stop; index += step) {
                    this.setBitTrue(index);
                }
                return;
            }
            console.log("    Using mask repetition optimization");
            const range_stop_word = range_stop >>> 5;
            for (let index = range_start; index < range_stop_unique; index += step) {
                let wordOffset = index >>> 5;
                const bitOffset = index & 31;
                const mask = (1 << bitOffset);
                do {
                    this.wordArray[wordOffset] |= mask;
                    wordOffset += step;
                } while (wordOffset <= range_stop_word);
            }
            return;
        }

        console.log("  Using small step path (optimized)");
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

// Test with a larger sieve to trigger the large-step path
const arr = new BitArray(1000);

// Simulate marking multiples of 17 (a larger prime that might use large-step path)
// factor = 8 (represents number 17: 2*8+1=17)
const factor = 8;
const step = factor * 2 + 1;  // = 17
const start = factor * factor * 2 + factor + factor;  // = 144
const stop = 500;

console.log("\nMarking multiples of 17:");
console.log("factor=", factor, "step=", step, "start=", start);
arr.setBitsTrue(start, step, stop);

// Check if 289 (17*17, index 144) is marked
console.log("\n289 (17*17, idx 144) should be marked:", arr.testBitTrue(144) ? "YES" : "NO (BUG!)");
console.log("323 (17*19, idx 161) should be marked:", arr.testBitTrue(161) ? "YES" : "NO (BUG!)");
