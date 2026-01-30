"use strict";

Promise.reject("Hello from Promise.reject").then(null, (message => console.log(message)));