"use strict";

const childProcess = require("child_process");

try {
    childProcess.fork("./Require_ChildProcess_Fork_MessagePassing_Child", [], { detached: true });
} catch (err) {
    console.log("detached error:", err.message.indexOf("detached child processes") >= 0);
}

try {
    childProcess.fork("./Require_ChildProcess_Fork_MessagePassing_Child", [], { serialization: "advanced" });
} catch (err) {
    console.log("serialization error:", err.message.indexOf("JSON message serialization") >= 0);
}

try {
    const isWindows = process.platform === "win32";
    childProcess.spawn(
        isWindows ? "cmd.exe" : "/bin/sh",
        isWindows ? ["/d", "/s", "/c", "exit /b 0"] : ["-c", "exit 0"],
        { stdio: ["pipe", "pipe", "pipe", "ipc"] });
} catch (err) {
    console.log("spawn ipc error:", err.message.indexOf("stdio[3]") >= 0 || err.message.indexOf("IPC") >= 0);
}
