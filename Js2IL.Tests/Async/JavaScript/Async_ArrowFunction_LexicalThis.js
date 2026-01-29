class Counter {
    constructor(initial) {
        this.x = initial;
    }

    makeGetter() {
        return async () => {
            console.log("Before await");
            await Promise.resolve(null);
            console.log("After await, this.x:", this.x);
            return this.x;
        };
    }
}

const c = new Counter(7);
const g = c.makeGetter();

const other = { x: 99, g };

other.g().then((value) => {
    console.log("Final value:", value);
});

console.log("After calling async getter");
