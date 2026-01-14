const r = Promise.withResolvers();

r.promise.then((value) => {
    console.log(value);
});

r.resolve("resolved");
