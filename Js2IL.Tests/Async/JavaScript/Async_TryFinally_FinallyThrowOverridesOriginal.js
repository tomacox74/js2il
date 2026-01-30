"use strict";

// exception thrown in finally should override original exception from try
async function test() {
    console.log("start");
    try {
        try {
            console.log("inner try: rejecting");
            await Promise.reject("original");
        } finally {
            console.log("finally: throwing");
            throw "finally-error";
        }
    } catch (e) {
        console.log("caught:", e);
    }
    console.log("end");
}

test().then(() => console.log("done"));
