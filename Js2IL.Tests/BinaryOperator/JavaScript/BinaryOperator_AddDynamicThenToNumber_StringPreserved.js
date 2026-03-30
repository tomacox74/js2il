"use strict";

class Accumulator {
    addAndCoerce(value) {
        let total = 4;
        total = total + value;
        return total;
    }
}

const accumulator = new Accumulator();
console.log(accumulator.addAndCoerce("2"));
console.log(accumulator.addAndCoerce(2));
