"use strict";

if (process.argv[2] === "emit-stdio") {
    console.log("silent child stdout");
    console.error("silent child stderr");
}

const fallbackExit = setTimeout(() => {
    process.exit(0);
}, 15000);

process.on("message", (message) => {
    if (message === "init") {
        process.send({ mode: process.argv[2] || "unknown" });
        return;
    }

    if (message === "shutdown") {
        clearTimeout(fallbackExit);
        process.disconnect();
        process.exit(0);
    }
});
