"use strict";

Promise.resolve("Hello from Promise.resolve").finally(() => console.log("[finally]", "Hello from finally")).then((message) => console.log("[then]", message));