"use strict";

const dense = [10, 20, 30];
Object.defineProperty(dense, "1", {
    value: 25,
    writable: false,
    enumerable: false,
    configurable: false
});
const denseDescriptor = Object.getOwnPropertyDescriptor(dense, "1");
console.log(dense[1], 1 in dense, Object.keys(dense).join(","));
console.log(denseDescriptor.value, denseDescriptor.writable, denseDescriptor.enumerable, denseDescriptor.configurable);
try {
    dense[1] = 99;
} catch (error) {
    console.log(error.name);
}
console.log(dense[1]);

let accessorValue = 4;
const accessor = [];
Object.defineProperty(accessor, "2", {
    get: function () { return accessorValue; },
    set: function (value) { accessorValue = value * 2; },
    enumerable: true,
    configurable: true
});
console.log(accessor.length, accessor[2], 2 in accessor);
accessor[2] = 7;
console.log(accessorValue, accessor[2]);
console.log(delete accessor[2], accessor.length, 2 in accessor);

const blocked = [0, 1, 2, 3];
Object.defineProperty(blocked, "2", { configurable: false });
try {
    Object.defineProperty(blocked, "length", { value: 1, writable: false });
} catch (error) {
    console.log(error.name);
}
const blockedLength = Object.getOwnPropertyDescriptor(blocked, "length");
console.log(blocked.length, 2 in blocked, 3 in blocked, blockedLength.writable);

const locked = [1];
Object.defineProperty(locked, "length", { writable: false });
try {
    locked[1] = 2;
} catch (error) {
    console.log(error.name);
}
locked[0] = 3;
console.log(locked.length, locked[0], 1 in locked);

const reflectLocked = [];
Object.defineProperty(reflectLocked, "length", { writable: false });
console.log(Reflect.defineProperty(reflectLocked, "0", {
    value: 1,
    writable: true,
    enumerable: true,
    configurable: true
}), reflectLocked.length, 0 in reflectLocked);
try {
    Reflect.defineProperty(reflectLocked, "length", { value: 1.5 });
} catch (error) {
    console.log(error.name);
}

const boundary = [];
Object.defineProperty(boundary, "4294967294", {
    value: "edge",
    writable: true,
    enumerable: true,
    configurable: true
});
console.log(boundary.length, boundary[4294967294]);
boundary[4294967295] = "named";
console.log(boundary.length, boundary[4294967295]);

const sparse = [];
sparse[2147483647] = "sparse";
console.log(sparse.length, sparse[2147483647], 2147483647 in sparse);
console.log(sparse.pop(), sparse.length, 2147483647 in sparse);

const maxPush = [];
maxPush.length = 4294967295;
try {
    maxPush.push("ordinary");
} catch (error) {
    console.log(error.name);
}
console.log(maxPush.length, maxPush[4294967295]);

const fixed = [1];
Object.preventExtensions(fixed);
fixed[0] = 2;
try {
    fixed[1] = 3;
} catch (error) {
    console.log(error.name);
}
console.log(fixed.length, fixed[0], 1 in fixed);

const sealed = [1, 2, 3];
Object.seal(sealed);
try {
    sealed.length = 1;
} catch (error) {
    console.log(error.name);
}
console.log(Object.isSealed(sealed), sealed.length, 2 in sealed);

const frozen = [5];
Object.freeze(frozen);
try {
    frozen[0] = 6;
} catch (error) {
    console.log(error.name);
}
try {
    frozen.length = 0;
} catch (error) {
    console.log(error.name);
}
try {
    frozen.push(7);
} catch (error) {
    console.log(error.name);
}
try {
    frozen.pop();
} catch (error) {
    console.log(error.name);
}
console.log(Object.isFrozen(frozen), frozen.length, frozen[0]);

const frozenEmpty = Object.freeze([]);
try {
    frozenEmpty.push();
} catch (error) {
    console.log(error.name);
}

function clearArray(value) {
    value.length = 0;
}
const dynamic = [1, 2];
clearArray(dynamic);
console.log(dynamic.length);

const proxyTarget = [1, 2];
const proxy = new Proxy(proxyTarget, {});
proxy.length = 1;
console.log(proxyTarget.length, 1 in proxyTarget);

const spliceProtected = [0, 1, 2];
Object.defineProperty(spliceProtected, "2", { configurable: false });
const spliceRemoved = spliceProtected.splice(1, 1, 9);
console.log(spliceProtected.join(","), spliceRemoved.join(","));

const sparseUnshift = [];
sparseUnshift.length = 3;
console.log(sparseUnshift.unshift("x"), sparseUnshift.length, 0 in sparseUnshift, 3 in sparseUnshift);

let keyConversions = 0;
const keyObject = {
    toString: function () {
        keyConversions++;
        return "blocked";
    }
};
try {
    Object.defineProperty(Object.preventExtensions({}), keyObject, { value: 1 });
} catch (error) {
    console.log(error.name, keyConversions);
}

const named = [];
named["01"] = "ordinary";
console.log(named.length, named["01"]);
