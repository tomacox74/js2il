class Counter {
    constructor() {
        this.count = 7;
    }

    async getCount() {
        await Promise.resolve(0);
        return this.count;
    }
}

const counter = new Counter();

counter.getCount().then((value) => {
    console.log("Count:", value);
});
