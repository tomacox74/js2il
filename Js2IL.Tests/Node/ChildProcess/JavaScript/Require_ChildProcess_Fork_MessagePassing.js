"use strict";

const childProcess = require("child_process");

if (false) {
    require("./Require_ChildProcess_Fork_MessagePassing_Child");
}

const child = childProcess.fork("./Require_ChildProcess_Fork_MessagePassing_Child", ["alpha"], {
    stdio: ["ignore", "pipe", "pipe", "ipc"],
    env: {
        JS2IL_FORK_TEST_MARKER: "parent-env"
    }
});

console.log("connected initially:", child.connected);
console.log("stdout piped:", child.stdout !== null);

let stdout = "";
let stderr = "";
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
        console.log("ready argv:", message.argv2);
        console.log("ready env:", message.env);
        console.log("send result:", child.send({ stage: "parent", value: 41 }));
        return;
    }

    console.log("reply stage:", message.stage);
    console.log("reply value:", message.value);
});

child.on("disconnect", () => {
    console.log("disconnect event:", true);
});

child.on("error", (err) => {
    console.log("error event:", err && err.message);
});

child.on("exit", (code, signal) => {
    console.log("exit code is null:", code === null);
    console.log("exit code raw:", code);
    console.log("exit signal:", signal);
});

child.on("close", (code, signal) => {
    console.log("close code is null:", code === null);
    console.log("close code raw:", code);
    console.log("close signal:", signal);
    console.log("stdout:", stdout.trim());
    console.log("stderr:", stderr.trim());
});
