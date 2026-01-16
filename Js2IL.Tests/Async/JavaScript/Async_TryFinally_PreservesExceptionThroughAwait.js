// exception thrown in try should be preserved through finally await
async function test() {
    console.log("start");
    try {
        try {
            console.log("inner try: rejecting");
            await Promise.reject("original");
        } finally {
            console.log("finally: before await");
            await Promise.resolve("cleanup");
            console.log("finally: after await");
        }
        console.log("inner after finally (should not print)");
    } catch (e) {
        console.log("caught:", e);
    }
    console.log("end");
}

test().then(() => console.log("done"));
