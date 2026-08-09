"use strict";

const { AsyncLocalStorage } = require("node:async_hooks");
const fs = require("node:fs");
const storage = new AsyncLocalStorage({ defaultValue: "none" });

storage.run("outer", () => {
    process.nextTick(() => console.log("nextTick:", storage.getStore()));
    Promise.resolve().then(() => console.log("promise:", storage.getStore()));
    setTimeout(() => console.log("timeout:", storage.getStore()), 0);
    setImmediate(() => console.log("immediate:", storage.getStore()));
    fs.readFile(__filename, "utf8", () => console.log("io:", storage.getStore()));
});

storage.run("sibling", () => {
    Promise.resolve().then(() => console.log("sibling:", storage.getStore()));
});

let resolveOutside;
const outside = new Promise(resolve => {
    resolveOutside = resolve;
});
outside.then(() => console.log("outside registration:", storage.getStore()));
storage.run("resolver", () => resolveOutside());

let resolveInside;
const inside = new Promise(resolve => {
    resolveInside = resolve;
});
storage.run("inside registration", () => {
    inside.then(() => console.log("inside registration:", storage.getStore()));
});
resolveInside();

console.log("sync:", storage.getStore());
