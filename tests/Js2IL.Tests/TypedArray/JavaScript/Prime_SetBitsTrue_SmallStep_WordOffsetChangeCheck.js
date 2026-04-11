"use strict";

class C {
	f() {
		let range_start = 4;
		let step = 3;
		let range_stop = 64;

		let index = range_start;
		let wordOffset = index >>> 5;

		index += step;
		const newwordOffset = index >>> 5;
		console.log("calc", wordOffset, newwordOffset);
		console.log("neq", (newwordOffset != wordOffset) ? 1 : 0);
	}
}

new C().f();
