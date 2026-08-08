"use strict";

if (process.argv[2] === "emit-stdio") {
    console.log("silent child stdout");
    console.error("silent child stderr");
}

const mode = process.argv[2] || "unknown";
const heartbeat = setInterval(() => {
    try {
        process.send({ mode: mode });
    } catch (_err) {
    }
}, 25);

setTimeout(() => {
    clearInterval(heartbeat);
    process.exit(0);
}, 15000);

process.on("message", (message) => {
    if (message === "shutdown") {
        clearInterval(heartbeat);
        process.disconnect();
        process.exit(0);
    }
});
