"use strict";

const childProcess = require("child_process");

const isWindows = process.platform === "win32";
const child = isWindows
    ? childProcess.spawn("cmd.exe", ["/d", "/s", "/c", "echo spawn-ignore"], { stdio: "ignore" })
    : childProcess.spawn("/bin/sh", ["-c", "printf spawn-ignore"], { stdio: "ignore" });

console.log("stdin ignored:", child.stdin === null);
console.log("stdout ignored:", child.stdout === null);
console.log("stderr ignored:", child.stderr === null);

child.on("close", (code) => {
    console.log("close code:", code);
});
