"use strict";

Promise.resolve(42).finally(() => Promise.resolve(999)).then(v => console.log("Result:", v));
