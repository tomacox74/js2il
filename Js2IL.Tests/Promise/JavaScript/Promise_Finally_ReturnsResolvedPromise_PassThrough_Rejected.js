Promise.reject("orig").finally(() => Promise.resolve(999)).catch(e => console.log(e));
