"use strict";

const parsed = new URL("https://example.com/docs?q=1#frag");
console.log("href:", parsed.href);
console.log("href string:", parsed.toString());
console.log("href json:", parsed.toJSON());
console.log("search q:", parsed.searchParams.get("q"));

const based = new URL("../guide", "https://example.com/docs/start/");
console.log("based href:", based.href);

const params = new URLSearchParams("a=1&a=2");
console.log("params a:", params.getAll("a").join("|"));
console.log("params string:", params.toString());

const url = require("url");
console.log("same URL:", globalThis.URL === url.URL);
console.log("same params:", globalThis.URLSearchParams === url.URLSearchParams);
console.log("url fn:", URL instanceof Function);
console.log("url proto:", Object.getPrototypeOf(URL) === Function.prototype);
console.log("url proto parent:", Object.getPrototypeOf(URL.prototype) === Object.prototype);
console.log("url instance proto:", Object.getPrototypeOf(parsed) === URL.prototype);
console.log("url instance ctor:", parsed.constructor === URL);
console.log("url instanceof:", parsed instanceof URL);
var urlPrototypeDescriptor = Object.getOwnPropertyDescriptor(URL, "prototype");
console.log("url descriptor writable:", urlPrototypeDescriptor.writable);
console.log("url descriptor enumerable:", urlPrototypeDescriptor.enumerable);
console.log("url descriptor configurable:", urlPrototypeDescriptor.configurable);
console.log("params fn:", URLSearchParams instanceof Function);
console.log("params proto:", Object.getPrototypeOf(URLSearchParams) === Function.prototype);
console.log("params proto parent:", Object.getPrototypeOf(URLSearchParams.prototype) === Object.prototype);
console.log("params instance proto:", Object.getPrototypeOf(params) === URLSearchParams.prototype);
console.log("params instance ctor:", params.constructor === URLSearchParams);
console.log("params instanceof:", params instanceof URLSearchParams);
var paramsPrototypeDescriptor = Object.getOwnPropertyDescriptor(URLSearchParams, "prototype");
console.log("params descriptor writable:", paramsPrototypeDescriptor.writable);
console.log("params descriptor enumerable:", paramsPrototypeDescriptor.enumerable);
console.log("params descriptor configurable:", paramsPrototypeDescriptor.configurable);

try {
    var urlCtor = URL;
    urlCtor("https://example.com/");
    console.log("url call: no-throw");
} catch (error) {
    console.log("url call:", error.name + ": " + error.message);
}

try {
    var paramsCtor = URLSearchParams;
    paramsCtor("a=1");
    console.log("params call: no-throw");
} catch (error) {
    console.log("params call:", error.name + ": " + error.message);
}
