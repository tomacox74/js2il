"use strict";

Promise.reject("Hello from Promise.reject").catch((message => console.log(message)));
