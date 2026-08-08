"use strict";

function target(value) {
    return this.base + value;
}

target.custom = 3;
let accessorValue = 4;
Object.defineProperty(target, "accessor", {
    get() {
        return accessorValue;
    },
    set(value) {
        accessorValue = value;
    },
    enumerable: true,
    configurable: true
});

const descriptor = Object.getOwnPropertyDescriptor(target, "accessor");
target.accessor = 8;
const spread = { ...target };
console.log(
    "reflection",
    typeof target,
    descriptor.get === Object.getOwnPropertyDescriptor(target, "accessor").get,
    descriptor.set === Object.getOwnPropertyDescriptor(target, "accessor").set,
    target.accessor,
    spread.custom,
    spread.accessor);
console.log(
    "keys",
    Object.keys(target).join(","),
    Object.getOwnPropertyNames(target).includes("name"),
    Object.getOwnPropertyNames(target).includes("length"));
Object.preventExtensions(target);
console.log("integrity", Object.isExtensible(target), Object.isExtensible(descriptor.get));
Object.freeze(descriptor.get);
console.log("frozenFunction", Object.isFrozen(descriptor.get));

function prototypeFunction() {}
const customFunctionPrototype = { marker: "custom-function-prototype" };
Object.setPrototypeOf(prototypeFunction, customFunctionPrototype);
console.log(
    "prototypeMutation",
    Object.getPrototypeOf(prototypeFunction) === customFunctionPrototype,
    prototypeFunction.marker);
console.log(
    "json",
    JSON.stringify(target) === undefined,
    JSON.stringify({ target }) === "{}");

const applyHandler = {
    apply(actualTarget, thisArgument, argumentsList) {
        return [
            actualTarget === target,
            this === applyHandler,
            thisArgument.base,
            argumentsList[0]
        ].join(":");
    }
};
const applyProxy = new Proxy(target, applyHandler);
const holder = { base: 10, method: applyProxy };
console.log(
    "apply",
    typeof applyProxy,
    holder.method(5),
    applyProxy instanceof target);

const fallbackProxy = new Proxy(target, {});
console.log("fallback", fallbackProxy.call({ base: 20 }, 2));

function Constructor(value) {
    this.value = value;
}

let constructProxy;
const constructHandler = {
    construct(actualTarget, argumentsList, newTarget) {
        return {
            targetMatches: actualTarget === Constructor,
            argument: argumentsList[0],
            newTargetMatches: newTarget === constructProxy
        };
    }
};
constructProxy = new Proxy(Constructor, constructHandler);
const constructed = new constructProxy(12);
console.log(
    "construct",
    typeof constructProxy,
    constructed.targetMatches,
    constructed.argument,
    constructed.newTargetMatches);

const invalidConstructProxy = new Proxy(Constructor, {
    construct() {
        return 1;
    }
});
let invalidConstructThrows = false;
try {
    new invalidConstructProxy();
} catch (error) {
    invalidConstructThrows = error instanceof TypeError;
}

const invalidApplyProxy = new Proxy(target, { apply: 1 });
let invalidApplyThrows = false;
try {
    invalidApplyProxy();
} catch (error) {
    invalidApplyThrows = error instanceof TypeError;
}
console.log("invalidTraps", invalidConstructThrows, invalidApplyThrows);

const integrityTarget = {};
const integrityHandler = {
    isExtensible(actualTarget) {
        return Object.isExtensible(actualTarget);
    },
    preventExtensions(actualTarget) {
        Object.preventExtensions(actualTarget);
        return true;
    }
};
const integrityProxy = new Proxy(integrityTarget, integrityHandler);
const extensibleBefore = Object.isExtensible(integrityProxy);
Object.preventExtensions(integrityProxy);
console.log(
    "proxyIntegrity",
    extensibleBefore,
    Object.isExtensible(integrityProxy),
    Object.isExtensible(integrityTarget));

let invalidExtensibleThrows = false;
try {
    Object.isExtensible(new Proxy({}, { isExtensible() { return false; } }));
} catch (error) {
    invalidExtensibleThrows = error instanceof TypeError;
}
let invalidPreventThrows = false;
try {
    Object.preventExtensions(new Proxy({}, { preventExtensions() { return true; } }));
} catch (error) {
    invalidPreventThrows = error instanceof TypeError;
}
console.log("proxyIntegrityInvariants", invalidExtensibleThrows, invalidPreventThrows);
