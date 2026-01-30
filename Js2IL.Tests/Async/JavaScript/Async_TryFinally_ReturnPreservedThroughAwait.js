"use strict";

// return value from try must be preserved when finally awaits and completes normally
async function test() {
    console.log("start");
    try {
        console.log("try: returning");
        return "try-return";
    } finally {
        console.log("finally: before await");
        await Promise.resolve("cleanup");
        console.log("finally: after await");
    }
}

test().then(v => {
    console.log("resolved:", v);
});
