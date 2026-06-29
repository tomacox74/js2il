"use strict";

const childProcess = require("child_process");

const isWindows = process.platform === "win32";
const file = isWindows ? "cmd.exe" : "/bin/sh";

// Test that stdout is returned as a string
const echoArgs = isWindows ? ["/c", "echo hello"] : ["-c", "printf hello"];
const output = childProcess.execFileSync(file, echoArgs, { encoding: "utf8" });
console.log("stdout contains hello:", output.indexOf("hello") >= 0);

// Test that non-zero exit code throws
try {
    const failArgs = isWindows ? ["/c", "exit 5"] : ["-c", "exit 5"];
    childProcess.execFileSync(file, failArgs, { encoding: "utf8" });
    console.log("expected error not thrown");
} catch (e) {
    console.log("throws on non-zero:", true);
    console.log("status:", e.status);
    console.log("code:", e.code);
    console.log("message has exit code:", e.message.indexOf("exit code 5") >= 0);
}
