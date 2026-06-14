"use strict";

const childProcess = require("child_process");

const command = process.platform === "win32"
    ? "echo exec-basic"
    : "printf exec-basic";

const child = childProcess.exec(command, (err, stdout, stderr) => {
    console.log("err is null:", err === null);
    console.log("stdout:", stdout.trim());
    console.log("stderr length:", stderr.length);
    console.log("kill after callback:", child.kill());
});

console.log("pid type:", typeof child.pid);
console.log("stdout piped:", child.stdout !== null);
