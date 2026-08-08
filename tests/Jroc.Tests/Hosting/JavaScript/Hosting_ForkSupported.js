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
        let readyReceived = false;
        const initTimer = setInterval(() => {
            if (readyReceived) {
                clearInterval(initTimer);
                return;
            }

            child.send({ stage: "init" });
        }, 25);

        child.on("message", (message) => {
            if (message.stage === "ready") {
                if (readyReceived) {
                    return;
                }

                readyReceived = true;
                clearInterval(initTimer);
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
            clearInterval(initTimer);
            reject(err);
        });

        child.on("close", (code, signal) => {
            clearInterval(initTimer);
            events.push("close:" + code + ":" + signal);
            resolve(events.join("|"));
        });
    });
};
