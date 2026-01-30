"use strict";\r\n\r\n// for await...of over a sync iterable (Array)
async function test() {
    let sum = 0;

    for await (const x of [1, 2, 3]) {
        console.log("x:", x);
        sum += x;
    }

    console.log("sum:", sum);
}

test().then(() => {
    console.log("done");
});
