// Test the mask repetition optimization path

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
        if (step > WORD_SIZE/2) { 
            let range_stop_unique =  range_start + 32 * step;
            if (range_stop_unique > range_stop) {
                for (let index = range_start; index < range_stop; index += step) {
                    this.setBitTrue(index);
                }
                return;
            }
            // This is the mask repetition path
            console.log("Using mask repetition: start=", range_start, "step=", step);
            console.log("  range_stop_unique=", range_stop_unique, "range_stop=", range_stop);
            const range_stop_word = range_stop >>> 5;
            console.log("  range_stop_word=", range_stop_word);
            
            for (let index = range_start; index < range_stop_unique; index += step) {
                let wordOffset = index >>> 5;
                const bitOffset = index & 31;
                const mask = (1 << bitOffset);
                console.log("  index=", index, "wordOffset=", wordOffset, "bitOffset=", bitOffset, "mask=", mask);
                
                do {
                    this.wordArray[wordOffset] |= mask;
                    wordOffset += step;
                } while (wordOffset <= range_stop_word);
            }
            return;
        }

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

// Create a large enough array to trigger mask repetition
const arr = new BitArray(100000);

// Use a step of 17 with a large range
const step = 17;
const start = 144;
const stop = 50000;

console.log("Testing mask repetition path:");
console.log("step=", step, "start=", start, "stop=", stop);
console.log("32 * step =", 32 * step, "so range_stop_unique =", start + 32 * step);

arr.setBitsTrue(start, step, stop);

// Verify a few values
console.log("\nVerifying:");
console.log("Index 144 marked?", arr.testBitTrue(144) ? "YES" : "NO");
console.log("Index 161 (144+17) marked?", arr.testBitTrue(161) ? "YES" : "NO");
console.log("Index 178 (144+34) marked?", arr.testBitTrue(178) ? "YES" : "NO");
console.log("Index 145 marked?", arr.testBitTrue(145) ? "YES (BUG!)" : "NO");
