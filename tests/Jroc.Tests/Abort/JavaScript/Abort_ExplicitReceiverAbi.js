"use strict";

const ac1 = new AbortController();
const signal1 = ac1.signal;
console.log(`aborted before: ${signal1.aborted}`);
AbortController.prototype.abort.call(ac1, "my-reason");
console.log(`aborted after: ${signal1.aborted}`);
console.log(`reason: ${signal1.reason}`);

const controllerPrototypeSignalDescriptor =
    Object.getOwnPropertyDescriptor(AbortController.prototype, "signal");
console.log(controllerPrototypeSignalDescriptor.get.call(ac1) === signal1);

const ac2 = new AbortController();
const signal2 = ac2.signal;
let listenerFired = false;
AbortSignal.prototype.addEventListener.call(signal2, "abort", () => { listenerFired = true; });
ac2.abort();
console.log(`listener fired after abort: ${listenerFired}`);

const ac3 = new AbortController();
const signal3 = ac3.signal;
let removedListenerFired = false;
const listener = () => { removedListenerFired = true; };
signal3.addEventListener("abort", listener);
AbortSignal.prototype.removeEventListener.call(signal3, "abort", listener);
ac3.abort();
console.log(`removed listener fired: ${removedListenerFired}`);

// Receiver errors.
try {
    AbortController.prototype.abort.call({});
} catch (error) {
    console.log(error instanceof TypeError);
}
try {
    controllerPrototypeSignalDescriptor.get.call({});
} catch (error) {
    console.log(error instanceof TypeError);
}
try {
    AbortSignal.prototype.addEventListener.call({}, "abort", () => {});
} catch (error) {
    console.log(error instanceof TypeError);
}
try {
    AbortSignal.prototype.removeEventListener.call({}, "abort", () => {});
} catch (error) {
    console.log(error instanceof TypeError);
}

const signalPrototypeAbortedDescriptor =
    Object.getOwnPropertyDescriptor(AbortSignal.prototype, "aborted");
const signalPrototypeReasonDescriptor =
    Object.getOwnPropertyDescriptor(AbortSignal.prototype, "reason");
console.log(signalPrototypeAbortedDescriptor.get.call(signal1));
console.log(signalPrototypeReasonDescriptor.get.call(signal1));
try {
    signalPrototypeAbortedDescriptor.get.call({});
} catch (error) {
    console.log(error instanceof TypeError);
}

console.log(AbortController.prototype.abort.length);
console.log(AbortSignal.prototype.addEventListener.length);
console.log(AbortSignal.prototype.removeEventListener.length);
