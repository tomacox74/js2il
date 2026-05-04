"use strict";

class Accumulator {
    run() {
        console.log(this.add(2, 3));
        console.log(this.add(10, 20));
    }

    add(a, b) {
        return a + b;
    }
}

new Accumulator().run();
