"use strict";

Promise.resolve(42).finally(() => Promise.reject("cleanup failed")).catch(e => console.log("Caught:", e));
