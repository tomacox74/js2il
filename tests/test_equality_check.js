// Test equality comparison with literal vs variable

const knownPrimeCounts = {
	1000000: 78498
};

const sieveSize = 1000000;
const countedPrimes = 78498;

const knownPrimeCount = knownPrimeCounts[sieveSize];

console.log("knownPrimeCount:", knownPrimeCount);
console.log("countedPrimes:", countedPrimes);
console.log("typeof knownPrimeCount:", typeof knownPrimeCount);
console.log("typeof countedPrimes:", typeof countedPrimes);
console.log("knownPrimeCount == countedPrimes:", knownPrimeCount == countedPrimes);
console.log("knownPrimeCount === countedPrimes:", knownPrimeCount === countedPrimes);

const validResult = (knownPrimeCount == countedPrimes);
console.log("validResult:", validResult);
