"use strict";

var o = { visible: 1 };
Object.preventExtensions(o);

Object.defineProperty(o, "visible", {
  value: 3
});

var visibleDesc = Object.getOwnPropertyDescriptor(o, "visible");
console.log("visible=" + o.visible);
console.log("visible_enum=" + visibleDesc.enumerable);
console.log("visible_config=" + visibleDesc.configurable);
console.log("visible_writable=" + visibleDesc.writable);

try {
  Object.defineProperty(o, "newProp", {
    value: 2
  });
  console.log("new_prop=allowed");
} catch (e) {
  console.log("new_prop=" + e.name);
}

console.log("has_new=" + Object.hasOwn(o, "newProp"));
