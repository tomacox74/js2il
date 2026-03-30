"use strict";

const events = require("node:events");
const EventEmitter = events.EventEmitter;

const emitter = new EventEmitter();
emitter.on(events.errorMonitor, function(err) {
  console.log("monitor:" + err.message);
});
emitter.on("error", function(err) {
  console.log("handler:" + err.message);
});

emitter.emit("error", new Error("boom"));

const emitter2 = new EventEmitter();
emitter2.on(events.errorMonitor, function(err) {
  console.log("monitor2:" + err);
});

try {
  emitter2.emit("error", "fatal");
  console.log("NO_THROW");
} catch (e) {
  console.log("thrown");
}
