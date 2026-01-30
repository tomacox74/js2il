"use strict";\r\n\r\n// await inside finally should run on normal completion
async function test() {
    console.log("try: before");
    try {
        console.log("try: inside");
        await Promise.resolve("ok");
        console.log("try: after await");
    } finally {
        console.log("finally: before await");
        const v = await Promise.resolve("finally-await");
        console.log("finally: after await", v);
    }
    console.log("after try/finally");
}

test().then(() => {
    console.log("done");
});

console.log("after call");
