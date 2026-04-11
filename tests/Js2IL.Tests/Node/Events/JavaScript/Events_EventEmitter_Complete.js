"use strict";

const events = require("node:events");
const EventEmitter = events.EventEmitter;

// Test eventNames()
const emitter1 = new EventEmitter();
console.log(emitter1.eventNames().length); // 0
emitter1.on("foo", function() {});
emitter1.on("bar", function() {});
const names = emitter1.eventNames();
console.log(names.length); // 2

// Test listeners()
const emitter2 = new EventEmitter();
console.log(emitter2.listeners("test").length); // 0
function listener1() {}
function listener2() {}
emitter2.on("test", listener1);
emitter2.on("test", listener2);
const testListeners = emitter2.listeners("test");
console.log(testListeners.length); // 2

// Test prependListener()
const emitter3 = new EventEmitter();
let order = "";
emitter3.on("event", function() {
  order += "a";
});
emitter3.prependListener("event", function() {
  order += "b";
});
emitter3.emit("event");
console.log(order); // ba

// Test prependOnceListener()
const emitter4 = new EventEmitter();
let prependOnceOrder = "";
emitter4.on("tick", function() {
  prependOnceOrder += "a";
});
emitter4.prependOnceListener("tick", function() {
  prependOnceOrder += "b";
});
emitter4.emit("tick");
console.log(prependOnceOrder); // ba
emitter4.emit("tick");
console.log(prependOnceOrder); // baa (only 'a' is added on second emit)

// Test setMaxListeners() and getMaxListeners()
const emitter5 = new EventEmitter();
console.log(emitter5.getMaxListeners()); // 10
emitter5.setMaxListeners(20);
console.log(emitter5.getMaxListeners()); // 20
emitter5.setMaxListeners(5);
console.log(emitter5.getMaxListeners()); // 5

// Test rawListeners()
const emitter6 = new EventEmitter();
emitter6.on("raw", function() {});
emitter6.once("raw", function() {});
const rawList = emitter6.rawListeners("raw");
console.log(rawList.length); // 2

// Test chaining
const emitter7 = new EventEmitter();
const result = emitter7
  .on("a", function() {})
  .prependListener("b", function() {})
  .setMaxListeners(15)
  .removeAllListeners();
console.log(result === emitter7); // true
console.log(emitter7.eventNames().length); // 0
console.log(emitter7.getMaxListeners()); // 15

// Test that listeners returns a copy
const emitter8 = new EventEmitter();
emitter8.on("copy", function() {});
const list1 = emitter8.listeners("copy");
const list2 = emitter8.listeners("copy");
console.log(list1 === list2); // false (different array instances)
console.log(list1.length); // 1
console.log(list2.length); // 1
