// Test that matches the actual PrimeJavaScript validation scenario
class Counter {
    countPrimes() {
        return 78498;
    }
}

const knownPrimeCounts = {
    1000000: 78498
};

const counter = new Counter();
const sieveSize = 1000000;
const countedPrimes = counter.countPrimes();
const knownPrimeCount = knownPrimeCounts[sieveSize];

console.log("knownPrimeCount:", knownPrimeCount);
console.log("countedPrimes:", countedPrimes);
console.log("Equal?", knownPrimeCount == countedPrimes);
