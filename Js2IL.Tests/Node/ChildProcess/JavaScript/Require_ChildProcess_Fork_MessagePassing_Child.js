"use strict";

const fallbackExit = setTimeout(() => {
    process.exit(0);
}, 500);

process.on("message", (message) => {
    clearTimeout(fallbackExit);
    console.log("child received:", message.value);
    console.log("reply sent:", process.send({ stage: "child", value: message.value + 1 }));
    process.disconnect();
    process.exit(0);
});

setTimeout(() => {
    console.log("child stdout alive");
    console.log("connected in child:", process.connected);
    console.log("ready sent:", process.send({
        stage: "ready",
        argv2: process.argv[2],
        env: process.env.JS2IL_FORK_TEST_MARKER
    }));
}, 100);
