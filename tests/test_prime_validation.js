// Test exact scenario from PrimeJavaScript.js

class PrimeSieve {
	constructor(sieveSize) {
		this.sieveSize = sieveSize;
	}
	
	countPrimes() {
		return 78498;
	}
	
	validatePrimeCount() {
		const knownPrimeCounts = {
			1000000: 78498
		};
		const countedPrimes = this.countPrimes();
		const knownPrimeCount = knownPrimeCounts[this.sieveSize];
		
		console.log("knownPrimeCount:", knownPrimeCount);
		console.log("countedPrimes:", countedPrimes);
		console.log("Equal?", knownPrimeCount == countedPrimes);
		
		const validResult = (knownPrimeCount == countedPrimes);
		return validResult;
	}
}

const sieve = new PrimeSieve(1000000);
const result = sieve.validatePrimeCount();
console.log("validResult:", result);
