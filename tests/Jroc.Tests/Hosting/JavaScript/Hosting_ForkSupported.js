"use strict";

const childProcess = require("child_process");

if (false) {
    require("./Hosting_ForkSupported_Child");
}

exports.startFork = function () {
    return new Promise((resolve, reject) => {
        const child = childProcess.fork("./Hosting_ForkSupported_Child", ["from-host"], {
            stdio: ["ignore", "ignore", "ignore", "ipc"],
            env: {
                HOSTING_FORK_MARKER: "env-ok"
            }
        });

        const events = [];

        child.on("message", (message) => {
            if (message.stage === "ready") {
                events.push("ready:" + message.argv2 + ":" + message.env);
                child.send({ stage: "parent", value: 41 });
                return;
            }

            events.push("reply:" + message.value);
        });

        child.on("disconnect", () => {
            events.push("disconnect:true");
        });

        child.on("error", (err) => {
            reject(err);
        });

        child.on("close", (code, signal) => {
            events.push("close:" + code + ":" + signal);
            resolve(events.join("|"));
        });
    });
};
