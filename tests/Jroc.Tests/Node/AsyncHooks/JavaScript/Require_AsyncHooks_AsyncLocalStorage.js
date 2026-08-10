"use strict";

const { AsyncLocalStorage, AsyncResource } = require("node:async_hooks");
const storage = new AsyncLocalStorage({ defaultValue: "none", name: "request" });
let captured;
let snapshot;

console.log("default:", storage.getStore());
console.log("name:", storage.name);
storage.run(undefined, () => {
    console.log("undefined store:", storage.getStore());
});

const result = storage.run("outer", function (value) {
    console.log("inside:", storage.getStore());
    captured = new AsyncResource("CAPTURED");
    return storage.run("inner", () => {
        console.log("nested:", storage.getStore());
        return value;
    });
}, 42);

console.log("result:", result);
console.log("restored:", storage.getStore());
console.log("captured:", captured.runInAsyncScope(() => storage.getStore()));

const bound = AsyncLocalStorage.bind(() => storage.getStore());
storage.run("snapshot", () => {
    snapshot = AsyncLocalStorage.snapshot();
});
storage.enterWith("entered");
console.log("bound:", bound());
console.log("snapshot:", snapshot(() => storage.getStore()));
console.log("exit:", storage.exit(() => storage.getStore()));
console.log("entered:", storage.getStore());
storage.disable();
console.log("disabled:", storage.getStore());
console.log("captured disabled:", captured.runInAsyncScope(() => storage.getStore()));
