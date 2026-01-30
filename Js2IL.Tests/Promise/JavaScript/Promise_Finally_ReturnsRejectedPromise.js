"use strict";\r\n\r\nPromise.resolve(42).finally(() => Promise.reject("cleanup failed")).catch(e => console.log("Caught:", e));
