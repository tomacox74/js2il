"use strict";\r\n\r\n// for await...of falls back to Symbol.iterator when Symbol.asyncIterator is missing
// Ensure IteratorClose/return() is called on break.
const iterable = {
    [Symbol.iterator]() {
        let i = 0;
        return {
            next: () => ({ value: i++, done: i > 3 }),
            return: () => {
                console.log("return called");
                return { done: true };
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
