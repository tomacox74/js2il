// Simplified test to check equality comparison in Prime validation scenario
const knownPrimeCounts = {
    1000000: 78498
};

const sieveSize = 1000000;
const countedPrimes = 78498;

const knownPrimeCount = knownPrimeCounts[sieveSize];
console.log("knownPrimeCount:", knownPrimeCount);
console.log("countedPrimes:", countedPrimes);
console.log("Equal?", knownPrimeCount == countedPrimes);
