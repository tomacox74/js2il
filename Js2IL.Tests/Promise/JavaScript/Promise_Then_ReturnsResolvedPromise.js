"use strict";\r\n\r\nPromise.resolve(1).then(x => Promise.resolve(x + 1)).then(v => console.log("Result:", v));
