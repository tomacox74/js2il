"use strict";\r\n\r\n// finally should run on rejection, and await inside finally must work
async function test() {
    console.log("before");
    try {
        console.log("try: before reject");
        await Promise.reject("error");
        console.log("try: after reject (should not print)");
    } catch (e) {
        console.log("catch:", e);
    } finally {
        console.log("finally: before await");
        await Promise.resolve("cleanup");
        console.log("finally: after await");
    }
    console.log("after");
}

test().then(() => console.log("done"));
