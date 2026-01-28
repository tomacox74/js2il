// for await...of using Symbol.asyncIterator, ensure IteratorClose/return() is called on break
const iterable = {
    [Symbol.asyncIterator]() {
        let i = 0;
        return {
            next: () => Promise.resolve({ value: i++, done: i > 3 }),
            return: () => {
                console.log("return called");
                return Promise.resolve({ value: "closed", done: true });
            }
        };
    }
};

async function test() {
    for await (const x of iterable) {
        console.log("x:", x);
        if (x === 1) {
            break;
        }
    }

    console.log("after");
}

test().then(() => {
    console.log("done");
});
