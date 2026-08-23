"use strict";

var setterCalls = 0;
var setterValue = "";

function SetterConstructor(value) {
    this.value = value;
}

Object.defineProperty(SetterConstructor.prototype, "value", {
    configurable: true,
    get: function () { return "getter:" + setterValue; },
    set: function (value) {
        setterCalls++;
        setterValue = value;
    }
});

var setterInstance = new SetterConstructor("setter");
console.log(
    setterCalls,
    setterValue,
    setterInstance.value,
    Object.prototype.hasOwnProperty.call(setterInstance, "value"));

function ReadOnlyConstructor(value) {
    this.value = value;
}

Object.defineProperty(ReadOnlyConstructor.prototype, "value", {
    configurable: true,
    enumerable: true,
    writable: false,
    value: "prototype-read-only"
});

try {
    new ReadOnlyConstructor("own");
    console.log("missing strict error");
} catch (error) {
    console.log(error.name);
}

function WritableConstructor(value) {
    this.value = value;
}

Object.defineProperty(WritableConstructor.prototype, "value", {
    configurable: true,
    enumerable: true,
    writable: true,
    value: "prototype-writable"
});

var writableInstance = new WritableConstructor("own");
console.log(
    writableInstance.value,
    WritableConstructor.prototype.value,
    Object.prototype.hasOwnProperty.call(writableInstance, "value"));

function ReadingConstructor(value) {
    this.value = value;
    console.log(this.value);
}

new ReadingConstructor("generated receiver");
ReadingConstructor.call({ value: "custom receiver" }, "updated custom receiver");

function PreInitConstructor(value) {
    this.value = value;
}

try {
    preInit.value;
    console.log("missing pre-initialization error");
} catch (error) {
    console.log(error.name);
}

var preInit = new PreInitConstructor("initialized");
console.log(preInit.value);
