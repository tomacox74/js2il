"use strict";\r\n\r\nPromise.reject("err").catch(e => Promise.resolve("recovered")).then(v => console.log("Result:", v));
