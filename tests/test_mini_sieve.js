// Mini prime sieve test - should find first few primes correctly

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
            for (let index = range_start; index < range_stop; index += step) {
                this.setBitTrue(index);
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

    searchBitFalse(index) {
        while (this.testBitTrue(index)) { 
            index++; 
        }
        return index;
    }
}

class PrimeSieve {
    constructor(sieveSize) {
        this.sieveSize = sieveSize;
        this.sieveSizeInBits = sieveSize >>> 1;
        this.bitArray = new BitArray(1 + this.sieveSizeInBits);
    }

    runSieve() {
        const q = Math.ceil(Math.sqrt(this.sieveSizeInBits));
        let factor = 1;

        while (factor < q) {
            const step = factor * 2 + 1;
            const start = factor * factor * 2 + factor + factor;
            this.bitArray.setBitsTrue(start, step, this.sieveSizeInBits);
            factor = this.bitArray.searchBitFalse(factor + 1);
        }
        return this;
    }

    getPrimes(max = 100) {
        const primes = [2];
        for (let factor = 1, count = 0; factor < this.sieveSizeInBits; factor++) {
            if (count >= max) break;
            if (!this.bitArray.testBitTrue(factor)) {
                count = primes.push(factor * 2 + 1);
            }
        }
        return primes;
    }
}

const sieve = new PrimeSieve(1000);
sieve.runSieve();
const primes = sieve.getPrimes(50);
console.log("First 50 primes up to 1000:", primes);
