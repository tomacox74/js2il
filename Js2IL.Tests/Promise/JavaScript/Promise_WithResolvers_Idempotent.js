"use strict";

const r = Promise.withResolvers();

r.promise
    .then((value) => console.log("then:" + value))
    .catch((reason) => console.log("catch:" + reason));

r.resolve("first");
r.reject("second");
