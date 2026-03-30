"use strict";

function logThrow(label, action) {
  try {
    const value = action();
    console.log(label + ":ok:" + value);
  } catch (error) {
    console.log(label + ":" + error.name);
  }
}

const target = { a: 1 };
const revocable = Proxy.revocable(target, {
  get: function (t, prop) {
    return t[prop];
  },
  set: function (t, prop, value) {
    t[prop] = value;
    return true;
  },
});

console.log(revocable.proxy.a);
revocable.proxy.b = 2;
console.log(target.b);
revocable.revoke();

logThrow("get", function () { return revocable.proxy.a; });
logThrow("set", function () { revocable.proxy.c = 3; return "set"; });
logThrow("has", function () { return "a" in revocable.proxy; });
logThrow("delete", function () { return delete revocable.proxy.a; });
logThrow("keys", function () { return Object.keys(revocable.proxy).join(","); });
logThrow("getPrototypeOf", function () { return Object.getPrototypeOf(revocable.proxy); });
logThrow("setPrototypeOf", function () { return Object.setPrototypeOf(revocable.proxy, null); });

const callable = Proxy.revocable(function (x) {
  return x + 1;
}, {});
callable.revoke();
logThrow("call", function () { return callable.proxy(1); });

const constructible = Proxy.revocable(function C(x) {
  this.x = x;
}, {});
constructible.revoke();
logThrow("construct", function () { return new constructible.proxy(1); });
