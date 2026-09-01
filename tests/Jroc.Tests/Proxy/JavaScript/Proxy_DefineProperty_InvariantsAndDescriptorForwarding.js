"use strict";

function throwsTypeError(callback) {
  try {
    callback();
    return false;
  } catch (error) {
    return error instanceof TypeError;
  }
}

let received;
const descriptor = { value: 1, configurable: true };
const proxy = new Proxy({}, {
  defineProperty: function (target, property, attributes) {
    received = attributes;
    return true;
  },
});

Object.defineProperty(proxy, "property", descriptor);
console.log(received !== descriptor);
console.log(Object.keys(received).sort().join(","));

let nestedReceived;
const nestedTarget = {};
const inner = new Proxy(nestedTarget, {
  defineProperty: function (target, property, attributes) {
    nestedReceived = attributes;
    return true;
  },
});
const outer = new Proxy(inner, {});
const nestedDescriptor = { value: 2, writable: true, configurable: true };

Object.defineProperty(outer, "property", nestedDescriptor);
console.log(nestedReceived !== nestedDescriptor);
console.log(Object.keys(nestedReceived).sort().join(","));

const nonExtensibleTarget = {};
Object.preventExtensions(nonExtensibleTarget);
const nonExtensibleProxy = new Proxy(nonExtensibleTarget, {
  defineProperty: function () {
    return true;
  },
});
console.log(throwsTypeError(function () {
  Object.defineProperty(nonExtensibleProxy, "newProperty", { value: 1 });
}));

const configurableTarget = {};
Object.defineProperty(configurableTarget, "property", { configurable: true });
const configurableProxy = new Proxy(configurableTarget, {
  defineProperty: function () {
    return true;
  },
});
console.log(throwsTypeError(function () {
  Object.defineProperty(configurableProxy, "property", { configurable: false });
}));

const fixedTarget = {};
Object.defineProperty(fixedTarget, "property", { value: 1 });
const fixedProxy = new Proxy(fixedTarget, {
  defineProperty: function () {
    return true;
  },
});
console.log(throwsTypeError(function () {
  Object.defineProperty(fixedProxy, "property", { value: 2 });
}));

const writableTarget = {};
const writableProxy = new Proxy(writableTarget, {
  defineProperty: function (target) {
    Object.defineProperty(target, "property", {
      configurable: false,
      writable: true,
    });
    return true;
  },
});
console.log(throwsTypeError(function () {
  Object.defineProperty(writableProxy, "property", { writable: false });
}));

const missingTarget = {};
const missingProxy = new Proxy(missingTarget, {
  defineProperty: function () {
    return true;
  },
});
console.log(throwsTypeError(function () {
  Object.defineProperty(missingProxy, "property", { configurable: false });
}));

let isExtensibleCalls = 0;
const nestedNonExtensibleTarget = {};
Object.preventExtensions(nestedNonExtensibleTarget);
const nestedNonExtensibleProxy = new Proxy(nestedNonExtensibleTarget, {
  isExtensible: function () {
    isExtensibleCalls++;
    return false;
  },
});
const nestedInvariantProxy = new Proxy(nestedNonExtensibleProxy, {
  defineProperty: function () {
    return true;
  },
});
console.log(throwsTypeError(function () {
  Object.defineProperty(nestedInvariantProxy, "property", { value: 1 });
}));
console.log(isExtensibleCalls);

const extensibilityOrder = [];
const extensibilityTarget = {};
const innerExtensibilityProxy = new Proxy(extensibilityTarget, {
  isExtensible: function () {
    extensibilityOrder.push("inner");
    return true;
  },
});
const outerExtensibilityProxy = new Proxy(innerExtensibilityProxy, {
  isExtensible: function () {
    extensibilityOrder.push("outer");
    return true;
  },
});
Object.isExtensible(outerExtensibilityProxy);
console.log(extensibilityOrder.join(","));

const descriptorTarget = {};
Object.preventExtensions(descriptorTarget);
const descriptorInnerProxy = new Proxy(descriptorTarget, {});
const descriptorOuterProxy = new Proxy(descriptorInnerProxy, {
  getOwnPropertyDescriptor: function () {
    return { value: 1, configurable: true };
  },
});
console.log(throwsTypeError(function () {
  Object.getOwnPropertyDescriptor(descriptorOuterProxy, "property");
}));

const nonConfigurableDeleteTarget = {};
Object.defineProperty(nonConfigurableDeleteTarget, "property", {
  configurable: false,
});
const nonConfigurableDeleteProxy = new Proxy(nonConfigurableDeleteTarget, {
  deleteProperty: function () {
    return true;
  },
});
console.log(throwsTypeError(function () {
  Reflect.deleteProperty(nonConfigurableDeleteProxy, "property");
}));

const nonExtensibleDeleteTarget = {};
Object.defineProperty(nonExtensibleDeleteTarget, "property", {
  configurable: true,
});
Object.preventExtensions(nonExtensibleDeleteTarget);
const nonExtensibleDeleteProxy = new Proxy(nonExtensibleDeleteTarget, {
  deleteProperty: function () {
    return true;
  },
});
console.log(throwsTypeError(function () {
  Reflect.deleteProperty(nonExtensibleDeleteProxy, "property");
}));

const writableDescriptorTarget = {};
Object.defineProperty(writableDescriptorTarget, "property", {
  configurable: false,
  writable: true,
});
const writableDescriptorProxy = new Proxy(writableDescriptorTarget, {
  getOwnPropertyDescriptor: function () {
    return {
      configurable: false,
      writable: false,
    };
  },
});
console.log(throwsTypeError(function () {
  Object.getOwnPropertyDescriptor(writableDescriptorProxy, "property");
}));

const descriptorOrder = [];
const descriptorSentinel = new Error("descriptor getter");
const orderedDescriptor = new Proxy({}, {
  has: function (target, property) {
    descriptorOrder.push("has:" + property);
    if (property === "configurable") {
      throw new Error("wrong abrupt completion");
    }
    return property === "enumerable";
  },
  get: function (target, property) {
    descriptorOrder.push("get:" + property);
    throw descriptorSentinel;
  },
});
try {
  Object.defineProperty({}, "property", orderedDescriptor);
} catch (error) {
  console.log(error === descriptorSentinel);
}
console.log(descriptorOrder.join(","));

const symbolKey = Symbol("key");
let definePropertyKey;
const symbolDefineProxy = new Proxy({}, {
  defineProperty: function (target, property) {
    definePropertyKey = property;
    return true;
  },
});
Object.defineProperty(symbolDefineProxy, symbolKey, { configurable: true });
console.log(definePropertyKey === symbolKey);

let deletePropertyKey;
const symbolDeleteProxy = new Proxy({}, {
  deleteProperty: function (target, property) {
    deletePropertyKey = property;
    return true;
  },
});
Reflect.deleteProperty(symbolDeleteProxy, symbolKey);
console.log(deletePropertyKey === symbolKey);

let absentIsExtensibleCalls = 0;
const absentInnerProxy = new Proxy({}, {
  isExtensible: function () {
    absentIsExtensibleCalls++;
    throw new Error("isExtensible must not be called");
  },
});
const absentOuterProxy = new Proxy(absentInnerProxy, {
  getOwnPropertyDescriptor: function () {
    return undefined;
  },
});
console.log(Object.getOwnPropertyDescriptor(absentOuterProxy, "missing") === undefined);
console.log(absentIsExtensibleCalls);

const preventedTarget = {};
Object.preventExtensions(preventedTarget);
const preventedInnerProxy = new Proxy(preventedTarget, {});
const preventedOuterProxy = new Proxy(preventedInnerProxy, {
  preventExtensions: function () {
    return true;
  },
});
console.log(Object.preventExtensions(preventedOuterProxy) === preventedOuterProxy);
