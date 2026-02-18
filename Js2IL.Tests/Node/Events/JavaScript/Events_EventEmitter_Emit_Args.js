"use strict";

const events = require("node:events");
const EventEmitter = events.EventEmitter;
const emitter = new EventEmitter();

let sum = 0;
let product = 0;

emitter.on("sum", function(a, b) {
  sum = a + b;
});

emitter.on("product", function(a, b, c) {
  product = a * b * c;
});

console.log(emitter.emit("sum", 2, 5));
console.log(sum);

console.log(emitter.emit("product", 2, 3, 4));
console.log(product);

console.log(emitter.emit("missing", 1, 2));
