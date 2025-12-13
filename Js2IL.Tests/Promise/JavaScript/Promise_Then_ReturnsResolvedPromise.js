Promise.resolve(1).then(x => Promise.resolve(x + 1)).then(v => console.log("Result:", v));
