"use strict";\r\n\r\nconst r = Promise.withResolvers();

r.promise.then((value) => {
    console.log(value);
});

r.resolve("resolved");
