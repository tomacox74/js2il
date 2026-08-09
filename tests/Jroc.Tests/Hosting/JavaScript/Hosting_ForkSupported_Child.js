"use strict";

const fallbackExit = setTimeout(() => {
    process.exit(0);
}, 2000);

process.on("message", (message) => {
    if (message && message.stage === "init") {
        process.send({
            stage: "ready",
            argv2: process.argv[2],
            env: process.env.HOSTING_FORK_MARKER
        });
        return;
    }

    if (!message || message.stage !== "parent") {
        return;
    }

    clearTimeout(fallbackExit);
    process.send({ stage: "child", value: message.value + 1 });
    process.disconnect();
    process.exit(0);
});
