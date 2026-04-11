"use strict";

const WORD_SIZE = 32;

class BitArray {
	constructor(size) {
		this.wordArray = new Int32Array(1 + (size >>> 5));
	}

	setBitTrue(index) {
		const wordOffset = index >>> 5;
		const bitOffset = index & 31;
		if (index < 100) {
			console.log("set", index, wordOffset, bitOffset);
		}
		this.wordArray[wordOffset] |= (1 << bitOffset);
	}

	setBitsTrue(range_start, step, range_stop) {
		if (step > WORD_SIZE / 2) {
			let range_stop_unique = range_start + 32 * step;
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
					wordOffset += step; // pattern repeats on word level after {step} words
				} while (wordOffset <= range_stop_word);
			}
			return;
		}

		// small-step path (unused in this test)
		for (let index = range_start; index < range_stop; index += step) {
			this.setBitTrue(index);
		}
	}

	setBitsTrue_Naive(range_start, step, range_stop) {
		console.log("naive", range_start, step, range_stop);
		for (let index = range_start; index < range_stop; index += step) {
			this.setBitTrue(index);
		}
	}
}

// Force large-step path: step > 16 and range_stop >= range_start + 32*step
const size = 2048;
const range_start = 1;
const step = 17;
const range_stop = 600;

const opt = new BitArray(size);
const naive = new BitArray(size);

console.log("types", typeof opt.wordArray, typeof naive.wordArray);
console.log("lens", opt.wordArray.length, naive.wordArray.length);

opt.setBitsTrue(range_start, step, range_stop);
naive.setBitsTrue_Naive(range_start, step, range_stop);

let diffs = 0;
let first = -1;
for (let i = 0; i < opt.wordArray.length; i++) {
	if (opt.wordArray[i] !== naive.wordArray[i]) {
		diffs++;
		if (first === -1) first = i;
	}
}

console.log("diffs", diffs);
if (first !== -1) {
	console.log("first", first, opt.wordArray[first], naive.wordArray[first]);
}

// Spot check a few words for debugging if needed
console.log("w0", opt.wordArray[0], naive.wordArray[0]);
console.log("w1", opt.wordArray[1], naive.wordArray[1]);
console.log("w2", opt.wordArray[2], naive.wordArray[2]);
console.log("w3", opt.wordArray[3], naive.wordArray[3]);
