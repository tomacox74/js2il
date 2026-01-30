"use strict";

Promise.resolve("Hello from Promise.resolve").then((message) => console.log("[then]", message)).finally(() => console.log("[finally]", "Hello from finally"));
