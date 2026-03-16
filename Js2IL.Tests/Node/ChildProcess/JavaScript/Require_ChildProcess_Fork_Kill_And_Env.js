"use strict";

const childProcess = require("child_process");

if (false) {
    require("./Require_ChildProcess_Fork_Kill_And_Env_Child");
}

const child = childProcess.fork("./Require_ChildProcess_Fork_Kill_And_Env_Child", ["from-parent"], {
    stdio: ["ignore", "pipe", "pipe", "ipc"],
    env: {
        JS2IL_KILL_MARKER: "env-ok"
    }
});

console.log("connected initially:", child.connected);

let stderr = "";
child.stderr.setEncoding("utf8");
child.stderr.on("data", (chunk) => {
    stderr += chunk;
});

child.on("message", (message) => {
    console.log("child env:", message.env);
    console.log("child argv:", message.argv2);
    console.log("kill result:", child.kill("SIGTERM"));
});

child.on("disconnect", () => {
    console.log("disconnect event:", true);
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
    console.log("stderr:", stderr.trim());
});
