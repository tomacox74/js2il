"use strict";\r\n\r\nconsole.log("Step 1");
Promise.resolve(42).finally(() => {
    console.log("Step 2: In finally");
    return Promise.reject("cleanup failed");
}).catch(e => {
    console.log("Step 3: Caught:", e);
});
console.log("Step 4");
