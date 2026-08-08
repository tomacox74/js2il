"use strict";

const { EventEmitter } = require("node:events");
const emitter = new EventEmitter();

let total = 0;
function listener(value) {
  total += value;
}

emitter.once("once", listener);
console.log("listeners-identity:" + (emitter.listeners("once")[0] === listener));
emitter.off("once", listener);
console.log("off-once-count:" + emitter.listenerCount("once"));
console.log("off-once-emitted:" + emitter.emit("once", 1));
console.log("total:" + total);

emitter.on("duplicate", listener);
emitter.on("duplicate", listener);
emitter.off("duplicate", listener);
console.log("duplicate-count:" + emitter.listenerCount("duplicate"));

let rawTotal = 0;
let rawCountDuringCall = -1;
function rawListener(value) {
  rawCountDuringCall = emitter.listenerCount("raw-once");
  rawTotal += value;
}

emitter.once("raw-once", rawListener);
const rawWrapper = emitter.rawListeners("raw-once")[0];
console.log("raw-listener:" + (rawWrapper.listener === rawListener));
rawWrapper(2);
rawWrapper(4);
console.log("raw-removal-count:" + rawCountDuringCall);
console.log("raw-count:" + emitter.listenerCount("raw-once"));
console.log("raw-total:" + rawTotal);

let duplicateOnceCalls = 0;
function duplicateOnceListener() {
  duplicateOnceCalls += 1;
}

emitter.once("duplicate-once", duplicateOnceListener);
emitter.once("duplicate-once", duplicateOnceListener);
const duplicateOnceListeners = emitter.listeners("duplicate-once");
console.log(
  "duplicate-once-identities:" +
    (duplicateOnceListeners[0] === duplicateOnceListener &&
      duplicateOnceListeners[1] === duplicateOnceListener),
);
emitter.removeListener("duplicate-once", duplicateOnceListener);
console.log(
  "duplicate-once-after-remove:" + emitter.listenerCount("duplicate-once"),
);
console.log(
  "duplicate-once-emitted:" + emitter.emit("duplicate-once"),
);
console.log("duplicate-once-calls:" + duplicateOnceCalls);
console.log(
  "duplicate-once-final-count:" + emitter.listenerCount("duplicate-once"),
);

async function asyncListener(value) {
  total += value;
}

function* generatorListener(value) {
  yield value;
}

emitter.on("async", asyncListener);
emitter.on("generator", generatorListener);
console.log("async-emitted:" + emitter.emit("async", 2));
console.log("generator-emitted:" + emitter.emit("generator", 3));
console.log("async-total:" + total);

try {
  emitter.off("generator", 42);
} catch (error) {
  console.log("error:" + error.message);
}
