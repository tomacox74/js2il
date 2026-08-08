"use strict";

const childProcess = require("node:child_process");
const command = process.platform === "win32"
  ? "echo callback-functions"
  : "printf callback-functions";

const generatorChild = childProcess.exec(command, function* generatorCallback() {
  yield "unused";
});
console.log("generator-pid:" + typeof generatorChild.pid);

childProcess.exec(command, async function asyncCallback(error, stdout, stderr) {
  console.log("async-error:" + (error === null));
  console.log("async-stdout:" + stdout.trim());
  console.log("async-stderr:" + stderr.length);
});
