"use strict";

const childProcess = require("child_process");

exports.attemptFork = function () {
    childProcess.fork("./unused-child");
};
