"use strict";

const fallbackExit = setTimeout(() => {
    process.exit(0);
}, 15000);

function safeSend(payload) {
    try {
        return process.send(payload);
    } catch (_err) {
        return false;
    }
}

let readySent = false;
let replied = false;

process.on("message", (message) => {
    if (message && message.stage === "init") {
        if (readySent) {
            return;
        }

        readySent = true;
        console.log("child stdout alive");
        console.log("connected in child:", process.connected);
        console.log("ready sent:", safeSend({
            stage: "ready",
            argv2: process.argv[2],
            env: process.env.JROC_FORK_TEST_MARKER
        }));
        return;
    }

    if (!message || message.stage !== "parent") {
        return;
    }

    if (replied) {
        return;
    }

    replied = true;
    clearTimeout(fallbackExit);
    console.log("child received:", message.value);
    console.log("reply sent:", safeSend({ stage: "child", value: message.value + 1 }));
    process.disconnect();
    process.exit(0);
});

setTimeout(() => {
    console.log("child stdout alive");
    process.exit(0);
}, 14000);
