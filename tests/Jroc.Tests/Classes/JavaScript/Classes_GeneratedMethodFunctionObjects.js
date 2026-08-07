"use strict";

class Base {
    label() {
        return "base";
    }
}

class Example extends Base {
    constructor(value) {
        super();
        this.value = value;
    }

    add(amount) {
        return super.label() + ":" + (this.value + amount);
    }

    get current() {
        return this.value;
    }

    set current(value) {
        this.value = value;
    }

    static identify(value) {
        return this.prefix + value;
    }
}

Example.prefix = "static:";

const instance = new Example(3);
const addDescriptor = Object.getOwnPropertyDescriptor(Example.prototype, "add");
const currentDescriptor = Object.getOwnPropertyDescriptor(Example.prototype, "current");
const staticDescriptor = Object.getOwnPropertyDescriptor(Example, "identify");

console.log("calls", instance.add(4), addDescriptor.value.call(instance, 5));
instance.current = 9;
console.log("accessors", instance.current, currentDescriptor.get.call(instance));
currentDescriptor.set.call(instance, 11);
console.log("setter", instance.current);
console.log("static", Example.identify("ok"), staticDescriptor.value.call(Example, "call"));
console.log(
    "metadata",
    addDescriptor.value.name,
    addDescriptor.value.length,
    currentDescriptor.get.name,
    currentDescriptor.set.name,
    staticDescriptor.value.name);
console.log(
    "descriptors",
    addDescriptor.value === Example.prototype.add,
    currentDescriptor.get === Object.getOwnPropertyDescriptor(Example.prototype, "current").get,
    Object.hasOwn(addDescriptor.value, "prototype"));

let constructThrows = false;
try {
    Reflect.construct(addDescriptor.value, []);
} catch (error) {
    constructThrows = error instanceof TypeError;
}
console.log("nonconstructor", constructThrows);

class Secret {
    #value;

    constructor(value) {
        this.#value = value;
    }

    read() {
        return this.#value;
    }
}

const secret = new Secret(17);
const read = Secret.prototype.read;
let privateThrows = false;
try {
    read.call({});
} catch (error) {
    privateThrows = error instanceof TypeError;
}
console.log("private", read.call(secret), privateThrows);

function makeClass() {
    return class {
        method() {
            return 1;
        }
    };
}

const First = makeClass();
const Second = makeClass();
console.log("identity", First.prototype.method !== Second.prototype.method);

function makeSecretClass() {
    return class {
        #value;

        constructor(value) {
            this.#value = value;
        }

        read() {
            return this.#value;
        }

        static #hidden() {
            return 23;
        }

        static readStatic() {
            return this.#hidden();
        }
    };
}

const FirstSecret = makeSecretClass();
const SecondSecret = makeSecretClass();
const firstSecret = new FirstSecret(1);
const secondSecret = new SecondSecret(2);
let crossBrandThrows = false;
try {
    FirstSecret.prototype.read.call(secondSecret);
} catch (error) {
    crossBrandThrows = error instanceof TypeError;
}
console.log(
    "privateIdentity",
    FirstSecret.prototype !== SecondSecret.prototype,
    FirstSecret.prototype.read !== SecondSecret.prototype.read,
    FirstSecret.prototype.read.call(firstSecret),
    crossBrandThrows);

let crossStaticBrandThrows = false;
try {
    FirstSecret.readStatic.call(SecondSecret);
} catch (error) {
    crossStaticBrandThrows = error instanceof TypeError;
}
console.log(
    "staticPrivateIdentity",
    FirstSecret.readStatic !== SecondSecret.readStatic,
    FirstSecret.readStatic(),
    crossStaticBrandThrows);
