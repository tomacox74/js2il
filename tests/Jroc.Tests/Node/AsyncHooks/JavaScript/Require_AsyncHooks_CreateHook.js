"use strict";

const {
    AsyncResource,
    createHook,
    executionAsyncResource
} = require("node:async_hooks");

const events = [];
const hook = createHook({
    init(id, type, triggerId, resource) {
        events.push("init:" + type + ":" + (resource instanceof AsyncResource));
    },
    before() {
        events.push("before");
    },
    after() {
        events.push("after");
    },
    destroy() {
        events.push("destroy");
    },
    promiseResolve() {
        events.push("promiseResolve");
    }
});

console.log("chain enable:", hook.enable() === hook);
const resource = new AsyncResource("HOOKED", { requireManualDestroy: true });
resource.runInAsyncScope(() => {
    events.push("resource:" + (executionAsyncResource() === resource));
});
resource.emitDestroy();
Promise.resolve("done").then(() => {
    events.push("promise callback");
});
setImmediate(() => {
    console.log("chain disable:", hook.disable() === hook);
    new AsyncResource("DISABLED");
    console.log(events.join(","));
});
