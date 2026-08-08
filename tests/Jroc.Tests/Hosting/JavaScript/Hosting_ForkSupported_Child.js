"use strict";

const fallbackExit = setTimeout(() => {
    process.exit(0);
}, 2000);

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
        safeSend({
            stage: "ready",
            argv2: process.argv[2],
            env: process.env.HOSTING_FORK_MARKER
        });
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
    safeSend({ stage: "child", value: message.value + 1 });
    process.disconnect();
    process.exit(0);
});
