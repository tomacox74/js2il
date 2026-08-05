"use strict";

const promise = import("node:path");
promise.then = () => console.log("override");
promise.then(() => console.log("direct"));
