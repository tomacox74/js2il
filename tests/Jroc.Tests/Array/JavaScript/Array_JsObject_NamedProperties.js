const values = [10, , undefined];
const marker = Symbol("marker");

values.custom = "named";
values[marker] = "symbol";

console.log(values.custom);
console.log(values[marker]);
console.log(values.length);
console.log(0 in values, 1 in values, 2 in values);
console.log(Object.keys(values).join(","));
console.log(Object.getOwnPropertyNames(values).join(","));
console.log(Object.getOwnPropertySymbols(values).length);
console.log(Object.getPrototypeOf(values) === Array.prototype);

const enumerableKeys = [];
for (const key in values) {
  enumerableKeys.push(key);
}
console.log(enumerableKeys.join(","));

Object.defineProperty(values, "hidden", {
  value: "descriptor",
  enumerable: false,
  configurable: true
});
console.log(values.hidden);
console.log(Object.keys(values).join(","));

const originalPrototype = Object.getPrototypeOf(values);
Object.setPrototypeOf(values, { inherited: "prototype" });
console.log(values.inherited);
Object.setPrototypeOf(values, originalPrototype);

const proxy = new Proxy(values, {});
proxy.viaProxy = "proxy";
console.log(values.viaProxy);

const identities = new WeakMap();
identities.set(values, "identity");
console.log(identities.get(values));
console.log(Object.getOwnPropertyNames(values).join(","));

delete values.custom;
console.log(values.custom);

values[5] = "sparse";
console.log(values.length, values[5], 4 in values);
values.length = 2;
console.log(values.length, 2 in values, 5 in values);

class DerivedArray extends Array {
  constructor() {
    super(2);
  }
}

const derived = new DerivedArray();
derived[1] = 42;
console.log(derived.length, derived[1]);

console.log(JSON.stringify(values));
console.log(values);

Object.preventExtensions(values);
console.log(Object.isExtensible(values));
