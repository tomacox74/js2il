"use strict";

const childProcess = require("child_process");

if (false) {
    require("./Require_ChildProcess_Fork_MessagePassing_Child");
}

const child = childProcess.fork("./Require_ChildProcess_Fork_MessagePassing_Child", ["alpha"], {
    stdio: ["ignore", "pipe", "pipe", "ipc"],
    env: {
        JROC_FORK_TEST_MARKER: "parent-env"
    }
});

console.log("connected initially:", child.connected);
console.log("stdout piped:", child.stdout !== null);

let stdout = "";
let stderr = "";
let readyReceived = false;
let replyReceived = false;
let replyValue = null;
let disconnectObserved = false;
child.stdout.setEncoding("utf8");
child.stdout.on("data", (chunk) => {
    stdout += chunk;
});
child.stderr.setEncoding("utf8");
child.stderr.on("data", (chunk) => {
    stderr += chunk;
});

child.on("message", (message) => {
    if (message.stage === "ready") {
        if (readyReceived) {
            return;
        }

        readyReceived = true;
        clearInterval(initTimer);
        console.log("ready argv:", message.argv2);
        console.log("ready env:", message.env);
        console.log("send result:", child.send({ stage: "parent", value: 41 }));
        return;
    }

    if (message.stage === "child") {
        replyReceived = true;
        replyValue = message.value;
    }
});

child.on("disconnect", () => {
    disconnectObserved = true;
});

child.on("error", (err) => {
    clearInterval(initTimer);
    console.log("error event:", err && err.message);
});

child.on("exit", (code, signal) => {
    console.log("exit code is null:", code === null);
    console.log("exit code raw:", code);
    console.log("exit signal:", signal);
});

child.on("close", (code, signal) => {
    clearInterval(initTimer);
    console.log("ready received:", readyReceived);
    console.log("reply received:", replyReceived);
    console.log("reply value:", replyValue);
    console.log("disconnect event:", disconnectObserved);
    console.log("close code is null:", code === null);
    console.log("close code raw:", code);
    console.log("close signal:", signal);
    console.log("stdout:", stdout.trim());
    console.log("stderr:", stderr.trim());
});

const initTimer = setInterval(() => {
    if (!readyReceived) {
        child.send({ stage: "init" });
    } else {
        clearInterval(initTimer);
    }
}, 25);

if (!readyReceived) {
    child.send({ stage: "init" });
}
