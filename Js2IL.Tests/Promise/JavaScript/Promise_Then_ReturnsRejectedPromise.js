"use strict";\r\n\r\nPromise.resolve(1).then(x => Promise.reject("error from then")).catch(e => console.log("Caught:", e));
