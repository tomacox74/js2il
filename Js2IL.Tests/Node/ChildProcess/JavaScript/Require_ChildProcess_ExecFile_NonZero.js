"use strict";

const childProcess = require("child_process");

const isWindows = process.platform === "win32";
const file = isWindows ? "cmd.exe" : "/bin/sh";
const args = isWindows
    ? ["/d", "/s", "/c", "exit /b 7"]
    : ["-c", "exit 7"];

const child = childProcess.execFile(file, args, (err, stdout, stderr) => {
    console.log("has error:", !!err);
    console.log("status:", err && err.status);
    console.log("code:", err && err.code);
    console.log("message has exit code:", err && err.message.indexOf("exit code 7") >= 0);
    console.log("stdout length:", stdout.length);
    console.log("stderr length:", stderr.length);
    console.log("kill after callback:", child.kill());
});
