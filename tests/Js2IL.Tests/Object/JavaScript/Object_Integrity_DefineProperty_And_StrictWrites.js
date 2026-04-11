"use strict";

var sealedValue = { a: 1 };
Object.seal(sealedValue);
Object.defineProperty(sealedValue, "a", { value: 2 });
console.log("sealed_value=" + sealedValue.a);
try {
  Object.defineProperty(sealedValue, "a", { configurable: true });
  console.log("sealed_reconfig=ok");
} catch (e) {
  console.log("sealed_reconfig=" + e.name);
}
try {
  Object.defineProperty(sealedValue, "b", { value: 1 });
  console.log("sealed_add=ok");
} catch (e) {
  console.log("sealed_add=" + e.name);
}

var frozenValue = { x: 1 };
Object.freeze(frozenValue);
try {
  Object.defineProperty(frozenValue, "x", { value: 2 });
  console.log("frozen_value_write=ok");
} catch (e) {
  console.log("frozen_value_write=" + e.name);
}
console.log("frozen_value=" + frozenValue.x);

var proto = {};
Object.defineProperty(proto, "blocked", {
  value: 1,
  writable: false,
  enumerable: true,
  configurable: true
});
var child = Object.create(proto);
try {
  child.blocked = 2;
  console.log("inherited_write=ok");
} catch (e) {
  console.log("inherited_write=" + e.name);
}
console.log("inherited_has_own=" + Object.hasOwn(child, "blocked"));
console.log("inherited_value=" + child.blocked);

var getterOnly = {};
Object.defineProperty(getterOnly, "value", {
  get: function () {
    return 7;
  },
  enumerable: true,
  configurable: true
});
try {
  getterOnly.value = 8;
  console.log("getter_only_write=ok");
} catch (e) {
  console.log("getter_only_write=" + e.name);
}
console.log("getter_only_value=" + getterOnly.value);
