// Test prime counting equality

class BitArray {
	constructor(size) {
		this.wordArray = new Int32Array(1 + (size >>> 5));
	}
	
	testBitTrue(index) {
		const wordOffset = index >>> 5;
		const bitOffset = index & 31;
		return this.wordArray[wordOffset] & (1 << bitOffset);
	}
}

class PrimeSieve {
	constructor(sieveSize) {
		this.sieveSize = sieveSize;
		this.sieveSizeInBits = sieveSize >>> 1;
		this.bitArray = new BitArray(1 + this.sieveSizeInBits);
	}
	
	countPrimes() {
		let total = 1;  // account for prime 2
		for (let index = 1; index < this.sieveSizeInBits; index++) {
			if (!this.bitArray.testBitTrue(index)) {
				total++;
			}
		}
		return total;
	}
}

const knownPrimeCounts = {
	1000000: 78498
};

const sieveSize = 1000000;
const sieve = new PrimeSieve(sieveSize);
const countedPrimes = sieve.countPrimes();
const knownPrimeCount = knownPrimeCounts[sieveSize];

console.log("knownPrimeCount:", knownPrimeCount);
console.log("countedPrimes:", countedPrimes);
console.log("typeof knownPrimeCount:", typeof knownPrimeCount);
console.log("typeof countedPrimes:", typeof countedPrimes);
console.log("knownPrimeCount == countedPrimes:", knownPrimeCount == countedPrimes);

const validResult = (knownPrimeCount == countedPrimes);
console.log("validResult:", validResult);
