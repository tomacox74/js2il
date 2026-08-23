"use strict";

const pc = require("picocolors");

module.exports = {
  red(value) { return pc.red(value); },
  green(value) { return pc.green(value); },
  yellow(value) { return pc.yellow(value); },
  cyan(value) { return pc.cyan(value); },
  bold(value) { return pc.bold(value); },
};
