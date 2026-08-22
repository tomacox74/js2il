"use strict";

function read(receiver) {
    return receiver.value;
}

function call(receiver) {
    return receiver.method();
}

function readLength(receiver) {
    return receiver.length;
}

function readMegamorphic(receiver) {
    return receiver.value;
}

function callMegamorphic(receiver) {
    return receiver.method();
}

const firstPrototype = {
    value: "prototype:first",
    method() {
        return "method:first:" + this.tag;
    }
};
const receiver = Object.create(firstPrototype);
receiver.tag = "receiver";

for (let index = 0; index < 3; index++) {
    read(receiver);
    call(receiver);
}

console.log(read(receiver));
console.log(call(receiver));

receiver.value = "own";
receiver.method = function () {
    return "method:own:" + this.tag;
};
console.log(read(receiver));
console.log(call(receiver));

delete receiver.value;
delete receiver.method;
console.log(read(receiver));
console.log(call(receiver));

let valueGetterCalls = 0;
let methodGetterCalls = 0;
Object.defineProperty(receiver, "value", {
    configurable: true,
    get() {
        valueGetterCalls++;
        return "accessor:" + valueGetterCalls;
    }
});
Object.defineProperty(receiver, "method", {
    configurable: true,
    get() {
        methodGetterCalls++;
        return function () {
            return "method:accessor:" + methodGetterCalls;
        };
    }
});
console.log(read(receiver));
console.log(read(receiver));
console.log(call(receiver));
console.log(call(receiver));

delete receiver.value;
delete receiver.method;
const secondPrototype = {
    value: "prototype:second",
    method() {
        return "method:second:" + this.tag;
    }
};
Object.setPrototypeOf(receiver, secondPrototype);
console.log(read(receiver));
console.log(call(receiver));

let proxyGetCalls = 0;
const proxy = new Proxy(receiver, {
    get(target, key) {
        proxyGetCalls++;
        if (key === "method") {
            return function () {
                return "method:proxy";
            };
        }
        return "proxy";
    }
});
console.log(read(proxy));
console.log(read(proxy));
console.log(call(proxy));
console.log(proxyGetCalls);

console.log(readLength("abc"));
console.log(readLength([1, 2]));

const megamorphicReceivers = [];
for (let index = 0; index < 5; index++) {
    const item = {
        value: "mega:value:" + index,
        method() {
            return "mega:method:" + index;
        }
    };
    megamorphicReceivers.push(item);
    console.log(readMegamorphic(item));
    console.log(callMegamorphic(item));
}

Object.defineProperty(megamorphicReceivers[0], "value", {
    configurable: true,
    get() {
        return "mega:accessor";
    }
});
megamorphicReceivers[0].method = function () {
    return "mega:method:updated";
};
console.log(readMegamorphic(megamorphicReceivers[0]));
console.log(callMegamorphic(megamorphicReceivers[0]));

const initiallyMissing = {};
console.log(read(initiallyMissing));
initiallyMissing.value = "appeared";
console.log(read(initiallyMissing));
console.log(receiver.__proto__ === secondPrototype);

try {
    read(null);
} catch (error) {
    console.log(error.name);
}

try {
    read(undefined);
} catch (error) {
    console.log(error.name);
}
