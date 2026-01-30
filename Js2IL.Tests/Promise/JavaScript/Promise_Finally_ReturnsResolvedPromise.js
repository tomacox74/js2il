"use strict";\r\n\r\nPromise.resolve(42).finally(() => Promise.resolve(999)).then(v => console.log("Result:", v));
