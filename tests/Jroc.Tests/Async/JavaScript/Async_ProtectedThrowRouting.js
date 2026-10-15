async function check(f, expected) {
    try {
        await f();
        throw new Error("Expected rejection");
    } catch (reason) {
        if (reason !== expected) throw new Error("Unexpected rejection: " + reason);
    }
}

async function run() {
    await check(async function() {
        try { throw "early"; } finally { throw "override"; }
        await 0;
    }, "override");
    await check(async function() {
        try {
            try { throw "early"; } finally { if (false) throw "unused"; }
        } finally {
            await 0;
            throw "outer";
        }
    }, "outer");
    await check(async function() {
        try { throw "early"; } finally { if (false) throw "unused"; }
    }, "early");
}

run().then(() => console.log("ok"), error => { throw error; });
