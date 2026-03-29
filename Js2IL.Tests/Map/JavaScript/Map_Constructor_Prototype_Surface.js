"use strict";

console.log(globalThis.Map === Map);
console.log(Map.prototype.constructor === Map);

var descriptor = Object.getOwnPropertyDescriptor(Map, "prototype");
console.log(descriptor.writable);
console.log(descriptor.enumerable);
console.log(descriptor.configurable);

var map = new Map();
console.log(Object.getPrototypeOf(map) === Map.prototype);
console.log(map instanceof Map);
console.log(map.constructor === Map);

Map.prototype.set.call(map, "answer", 42);
console.log(Map.prototype.get.call(map, "answer"));

try {
  var mapCtor = Map;
  mapCtor();
  console.log("no-throw");
} catch (e) {
  console.log(e.name + ": " + e.message);
}
