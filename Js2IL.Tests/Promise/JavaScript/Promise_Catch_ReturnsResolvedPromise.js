"use strict";

Promise.reject("err").catch(e => Promise.resolve("recovered")).then(v => console.log("Result:", v));
