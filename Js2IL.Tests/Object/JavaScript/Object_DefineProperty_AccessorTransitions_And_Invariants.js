"use strict";

var o = {};
var log = [];

function getter() {
  return 17;
}

function setter1(v) {
  log.push("s1:" + v);
}

function setter2(v) {
  log.push("s2:" + v);
}

Object.defineProperty(o, "x", {
  get: getter,
  set: setter1,
  enumerable: true,
  configurable: true
});

Object.defineProperty(o, "x", {
  set: setter2
});

o.x = 9;
var xDesc = Object.getOwnPropertyDescriptor(o, "x");
console.log("get=" + o.x);
console.log("log=" + log.join(","));
console.log("same_get=" + (xDesc.get === getter));
console.log("same_set=" + (xDesc.set === setter2));
console.log("enum=" + xDesc.enumerable);
console.log("config=" + xDesc.configurable);

try {
  Object.defineProperty(o, "mixed", {
    value: 1,
    get: getter
  });
  console.log("mixed=allowed");
} catch (e) {
  console.log("mixed=" + e.name);
}

var locked = {};
Object.defineProperty(locked, "y", {
  value: 1,
  enumerable: true,
  configurable: false,
  writable: false
});

try {
  Object.defineProperty(locked, "y", {
    value: 2
  });
  console.log("locked_value=allowed");
} catch (e) {
  console.log("locked_value=" + e.name);
}

try {
  Object.defineProperty(locked, "y", {
    get: getter
  });
  console.log("locked_kind=allowed");
} catch (e) {
  console.log("locked_kind=" + e.name);
}

var yDesc = Object.getOwnPropertyDescriptor(locked, "y");
console.log("locked_final=" + yDesc.value + "," + yDesc.writable + "," + yDesc.configurable);

try {
  Object.defineProperty({}, "primitive", "not-an-object");
  console.log("primitive=allowed");
} catch (e) {
  console.log("primitive=" + e.name);
}

var inheritedAttributes = Object.create({
  enumerable: true,
  configurable: true,
  value: 33
});
var inheritedTarget = {};
Object.defineProperty(inheritedTarget, "proto", inheritedAttributes);
var inheritedDesc = Object.getOwnPropertyDescriptor(inheritedTarget, "proto");
console.log("proto_value=" + inheritedTarget.proto);
console.log("proto_enum=" + inheritedDesc.enumerable);
console.log("proto_config=" + inheritedDesc.configurable);
