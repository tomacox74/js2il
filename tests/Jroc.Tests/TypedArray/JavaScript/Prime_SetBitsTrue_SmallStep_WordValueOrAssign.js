"use strict";

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
		console.log("args", range_start, step, range_stop);
		console.log("cmp0", (range_start < range_stop) ? 1 : 0);
		if (step > WORD_SIZE / 2) {
			throw new Error("not used in this test");
		}

		let index = range_start;
		let wordOffset = index >>> 5;
		let wordValue = this.wordArray[wordOffset];
		console.log("init", index, wordOffset, wordValue);

		while (index < range_stop) {
			const bitOffset = index & 31;
			wordValue |= (1 << bitOffset);
			if (index === range_start) {
				console.log("iter0", index, bitOffset, wordValue);
			}

			index += step;
			const newwordOffset = index >>> 5;
			if (newwordOffset != wordOffset) {
				console.log("commit", wordOffset, wordValue, "->", newwordOffset);
				this.wordArray[wordOffset] = wordValue;
				wordOffset = newwordOffset;
				wordValue = this.wordArray[wordOffset];
			}
		}

		console.log("end", index, wordOffset, wordValue);
		this.wordArray[wordOffset] = wordValue;
		console.log("after", this.wordArray[0], this.wordArray[1], this.wordArray[2]);
	}

	testBitTrue(index) {
		const wordOffset = index >>> 5;
		const bitOffset = index & 31;
		return this.wordArray[wordOffset] & (1 << bitOffset);
	}
}

const b = new BitArray(64);
const t = new Int32Array(3);
let idx = 0;
t[idx] = 16;
console.log("varIndexSet", t[0]);
// start=4 corresponds to number 9 in PrimeJavaScript mapping
b.setBitsTrue(4, 3, 64);

console.log("bit4", b.testBitTrue(4) ? 1 : 0);
console.log("bit7", b.testBitTrue(7) ? 1 : 0);
console.log("bit31", b.testBitTrue(31) ? 1 : 0);
console.log("bit5", b.testBitTrue(5) ? 1 : 0);
console.log("word0", b.wordArray[0]);
console.log("word1", b.wordArray[1]);
