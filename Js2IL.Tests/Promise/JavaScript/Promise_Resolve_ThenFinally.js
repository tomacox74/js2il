"use strict";\r\n\r\nPromise.resolve("Hello from Promise.resolve").then((message) => console.log("[then]", message)).finally(() => console.log("[finally]", "Hello from finally"));
