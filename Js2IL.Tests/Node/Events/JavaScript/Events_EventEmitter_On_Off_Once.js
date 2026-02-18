"use strict";

const events = require("node:events");
const EventEmitter = events.EventEmitter;

const emitter = new EventEmitter();

let sum = 0;
function onData() {
  sum += 1;
}

console.log(emitter.emit("data"));
emitter.on("data", onData);
console.log(emitter.emit("data"));
console.log(sum);
console.log(emitter.listenerCount("data"));
emitter.off("data", onData);
console.log(emitter.emit("data"));
console.log(sum);

let onceTotal = 0;
emitter.once("tick", function() {
  onceTotal += 1;
});
console.log(emitter.listenerCount("tick"));
console.log(emitter.emit("tick"));
console.log(emitter.emit("tick"));
console.log(onceTotal);

emitter.on("alpha", function() {});
emitter.on("beta", function() {});
emitter.removeAllListeners("alpha");
console.log(emitter.listenerCount("alpha"));
emitter.removeAllListeners();
console.log(emitter.listenerCount("beta"));
