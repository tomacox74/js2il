"use strict";

const someObject = () => 42;

if (typeof someObject === "function") {
    console.log("function");
}

if (typeof 123 === "function") {
    console.log("unexpected number function");
} else {
    console.log("not function");
}

if (typeof someObject !== "function") {
    console.log("unexpected");
} else {
    console.log("still function");
}
