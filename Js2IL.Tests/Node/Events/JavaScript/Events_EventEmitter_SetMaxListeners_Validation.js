"use strict";

const events = require("node:events");
const EventEmitter = events.EventEmitter;

// Test setMaxListeners with valid values
const emitter1 = new EventEmitter();
console.log(emitter1.getMaxListeners()); // 10
emitter1.setMaxListeners(20);
console.log(emitter1.getMaxListeners()); // 20
emitter1.setMaxListeners(0); // 0 is valid (means unlimited)
console.log(emitter1.getMaxListeners()); // 0

// Test setMaxListeners with string that can be converted to number
const emitter2 = new EventEmitter();
emitter2.setMaxListeners("15");
console.log(emitter2.getMaxListeners()); // 15

// Test setMaxListeners with negative number (should throw)
const emitter3 = new EventEmitter();
try {
  emitter3.setMaxListeners(-5);
  console.log("SHOULD NOT REACH HERE");
} catch (e) {
  console.log("Caught error for negative value"); // Expected
}

// Test setMaxListeners with NaN (should throw)
const emitter4 = new EventEmitter();
try {
  emitter4.setMaxListeners("invalid");
  console.log("SHOULD NOT REACH HERE");
} catch (e) {
  console.log("Caught error for NaN"); // Expected
}
