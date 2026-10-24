"use strict";

const before = process.cwd();
const fs = require("fs");
const path = require("path");
const marker = "jroc-chdir-" + Date.now() + ".txt";
const markerPath = path.join(__dirname, marker);
fs.writeFileSync(markerPath, "cwd marker");
try {
  process.chdir(__dirname);
  console.log("cwd changed", fs.readFileSync(marker, "utf8") === "cwd marker");
} finally {
  process.chdir(before);
  fs.unlinkSync(markerPath);
}
console.log("cwd restored", process.cwd() === before);

const order = [];
process.nextTick(() => order.push("tick"));
order.push("sync");
setImmediate(() => {
  order.push("immediate");
  console.log("order", order.join(","));
});
