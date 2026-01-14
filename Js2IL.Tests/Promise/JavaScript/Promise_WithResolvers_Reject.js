const r = Promise.withResolvers();

r.promise.catch((reason) => {
    console.log(reason);
});

r.reject("rejected");
