"use strict";\r\n\r\nconst r = Promise.withResolvers();

r.promise
    .then((value) => console.log("then:" + value))
    .catch((reason) => console.log("catch:" + reason));

r.resolve("first");
r.reject("second");
