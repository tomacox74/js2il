"use strict";

if (process.argv[2] === "emit-stdio") {
    console.log("silent child stdout");
    console.error("silent child stderr");
}

process.on("message", (message) => {
    if (message === "shutdown") {
        process.disconnect();
    }
});

process.send({ mode: process.argv[2] || "unknown" });
