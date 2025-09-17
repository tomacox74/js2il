/* js2il-compatible variant to avoid nested shift+add codegen bug in constructor */
"use strict";
const NOW_UNITS_PER_SECOND = 1000;
const WORD_SIZE = 32;

let config = {
	sieveSize: 1000000,
	timeLimitSeconds: 5,
	verbose: false,
	runtime: ''
};

const { performance } = require('perf_hooks');
const runtimeParts = process.argv[0].split(/[\\\/]/);
config.runtime = runtimeParts[runtimeParts.length - 1];
config.verbose = process.argv.includes("verbose");

class BitArray
{
	constructor(size)
	{
		// Avoid nested (1 + (size >>> 5)) which currently miscompiles in js2il
		const words = size >>> 5;
		const capacity = 1 + words;
		this.wordArray = new Int32Array(capacity);
	}

	setBitTrue(index)
	{
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

	testBitTrue(index)
	{
		const wordOffset = index >>> 5;
		const bitOffset = index & 31;
		return this.wordArray[wordOffset] & (1 << bitOffset);
	}

	searchBitFalse(index)
	{
		while (this.testBitTrue(index)) { index++ };
		return index;
	}
}

class PrimeSieve
{
	constructor(sieveSize)
	{
		this.sieveSize = sieveSize;
		this.sieveSizeInBits = sieveSize >>> 1;
		this.bitArray = new BitArray(1 + this.sieveSizeInBits);
	}

	runSieve()
	{
		const q = Math.ceil(Math.sqrt(this.sieveSizeInBits));
		let factor = 1;

		while (factor < q)
		{
			const step = factor * 2 + 1;
			const start = factor * factor * 2 + factor + factor;

			this.bitArray.setBitsTrue(start, step, this.sieveSizeInBits);
			factor = this.bitArray.searchBitFalse(factor + 1);
		}
		return this;
	}

	countPrimes()
	{
		let total = 1;
		for (let index = 1; index < this.sieveSizeInBits; index++)
		{
			if (!this.bitArray.testBitTrue(index))
			{
				total++;
			}
		}
		return total;
	}

	getPrimes(max = 100)
	{
		const primes = [2];
		for (let factor = 1, count = 0; factor < this.sieveSizeInBits; factor++)
		{
			if (count >= max) break;
			if (!this.bitArray.testBitTrue(factor)) count = primes.push(factor * 2 + 1);
		}
		return primes;
	}

	validatePrimeCount(verbose) 
	{
		const maxShowPrimes = 100;
		const knownPrimeCounts = {
			10: 4,
			100: 25,
			1000: 168,
			10000: 1229,
			100000: 9592,
			1000000: 78498,
			10000000: 664579,
			100000000: 5761455
		};
		const countedPrimes = this.countPrimes();
		const primeArray = this.getPrimes(maxShowPrimes);

		let validResult = false;
		if (this.sieveSize in knownPrimeCounts)
		{
			const knownPrimeCount = knownPrimeCounts[this.sieveSize];
			validResult = (knownPrimeCount == countedPrimes);
			if (!validResult)
				console.log(
					"\nError: invalid result.",
					`Limit for ${this.sieveSize} should be ${knownPrimeCount} `,
					`but result contains ${countedPrimes} primes`
				);
		}
		else console.log(
			`Warning: cannot validate result of ${countedPrimes} primes:`,
			`limit ${this.sieveSize} is not in the known list of number of primes!`
		);

		if (verbose)
			console.log(`\nThe first ${maxShowPrimes} found primes are:`, primeArray);
	
		return validResult;
	}
}

const runSieveBatch = (sieveSize, timeLimitSeconds = 5) =>
{
	let nrOfPasses = 0;
	const timeStart = performance.now();
	const timeFinish = timeStart + timeLimitSeconds * NOW_UNITS_PER_SECOND;

	do
	{
		const sieve = new PrimeSieve(sieveSize);
		sieve.runSieve();
		nrOfPasses++;
	}
	while (performance.now() < timeFinish);

	return nrOfPasses;
}

const main = ({ sieveSize, timeLimitSeconds, verbose, runtime }) =>
{
	const validResult = new PrimeSieve(sieveSize).runSieve().validatePrimeCount(verbose);
	if (!validResult) return false;

	const timeStart = performance.now();
	const totalPasses = runSieveBatch(sieveSize, timeLimitSeconds);
	const timeEnd = performance.now();
	const durationInSec = (timeEnd - timeStart) / NOW_UNITS_PER_SECOND;
	console.log(`\nrogiervandam-${runtime};${totalPasses};${durationInSec};1;algorithm=base,faithful=yes,bits=1`); 
}

main(config);
