"use strict";\r\n\r\n// Test equality comparison between object property value and method return value
// This reproduces the bug found in PrimeJavaScript.js where validation fails

class Counter {
	count() {
		let total = 0;
		for (let i = 0; i < 10; i++) {
			total++;
		}
		return total;
	}
}

const expectedCounts = {
	10: 10,
	100: 100
};

const counter = new Counter();
const counted = counter.count();
const expected = expectedCounts[10];

console.log(counted);
console.log(expected);
console.log(counted == expected);
