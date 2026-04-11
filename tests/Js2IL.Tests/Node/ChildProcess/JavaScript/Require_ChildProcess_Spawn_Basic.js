"use strict";

const childProcess = require("child_process");

const isWindows = process.platform === "win32";
const child = isWindows
    ? childProcess.spawn("cmd.exe", ["/d", "/s", "/c", "echo spawn-basic"])
    : childProcess.spawn("/bin/sh", ["-c", "printf spawn-basic"]);

console.log("pid type:", typeof child.pid);
console.log("stdout piped:", child.stdout !== null);
console.log("stderr piped:", child.stderr !== null);

let stdout = "";
let stderr = "";

child.stdout.on("data", (chunk) => {
    stdout += chunk;
});

child.stderr.on("data", (chunk) => {
    stderr += chunk;
});

child.on("exit", (code) => {
    console.log("exit code:", code);
});

child.on("close", (code) => {
    console.log("close code:", code);
    console.log("stdout:", stdout.trim());
    console.log("stderr length:", stderr.length);
    console.log("kill after close:", child.kill());
});
