"use strict";

const asyncHooks = require("node:async_hooks");
const legacyAsyncHooks = require("async_hooks");
const resource = new asyncHooks.AsyncResource("JROC_TEST");
const receiver = { value: 7 };
const outerId = asyncHooks.executionAsyncId();

const result = resource.runInAsyncScope(function (left, right) {
    console.log("receiver:", this === receiver);
    console.log("resource id:", asyncHooks.executionAsyncId() === resource.asyncId());
    console.log("trigger id:", asyncHooks.triggerAsyncId() === resource.triggerAsyncId());
    return this.value + left + right;
}, receiver, 2, 3);

console.log("same module:", asyncHooks === legacyAsyncHooks);
console.log("constructor:", typeof asyncHooks.AsyncResource);
console.log("result:", result);
const bound = resource.bind(function (value) {
    return this.value + value;
}, receiver);
console.log("bound:", bound(5));
const staticBound = asyncHooks.AsyncResource.bind(function () {
    return this.value;
}, "STATIC_TEST", receiver);
console.log("static bound:", staticBound());
try {
    resource.runInAsyncScope(() => {
        throw new Error("boom");
    });
} catch (error) {
    console.log("error:", error.message);
}
console.log("restored:", asyncHooks.executionAsyncId() === outerId);
