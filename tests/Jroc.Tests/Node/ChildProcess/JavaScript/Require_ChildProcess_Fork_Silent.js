"use strict";

const childProcess = require("child_process");

if (false) {
    require("./Require_ChildProcess_Fork_Silent_Child");
}

function runSilentTrue() {
    const child = childProcess.fork("./Require_ChildProcess_Fork_Silent_Child", ["emit-stdio"], { silent: true });
    console.log("silent true stdout piped:", child.stdout !== null);
    console.log("silent true stderr piped:", child.stderr !== null);
    console.log("silent true connected initially:", child.connected);

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
        console.log("silent true message:", message.mode);
        console.log("silent true send:", child.send("shutdown"));
    });

    child.on("close", (code) => {
        console.log("silent true close:", code);
        console.log("silent true stdout marker:", stdout.indexOf("silent child stdout") >= 0);
        console.log("silent true stderr marker:", stderr.indexOf("silent child stderr") >= 0);
        runSilentFalse();
    });
}

function runSilentFalse() {
    const child = childProcess.fork("./Require_ChildProcess_Fork_Silent_Child", ["quiet"], { silent: false });
    console.log("silent false stdout null:", child.stdout === null);
    console.log("silent false stderr null:", child.stderr === null);
    console.log("silent false connected initially:", child.connected);

    child.on("message", (message) => {
        console.log("silent false message:", message.mode);
        console.log("silent false send:", child.send("shutdown"));
    });

    child.on("close", (code) => {
        console.log("silent false close:", code);
    });
}

runSilentTrue();
