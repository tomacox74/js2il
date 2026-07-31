"use strict";

async function test(value) {
    const before = value + 1;
    const resumed = await Promise.resolve(10);
    console.log(before + resumed);
}

test(4);
