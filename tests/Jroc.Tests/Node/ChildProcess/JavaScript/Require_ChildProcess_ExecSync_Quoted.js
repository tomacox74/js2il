"use strict";

const childProcess = require("child_process");

const output = childProcess.execSync(
    "node -e \"process.stdout.write('quoted-ok')\"",
    { encoding: "utf8" }
);

console.log("quoted output ok:", output === "quoted-ok");
