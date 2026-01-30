"use strict";\r\n\r\nPromise.allSettled(null)
    .then((result) => {
        console.log("Should not reach here");
    })
    .catch((error) => {
        console.log("Caught error type:", error.name);
    });
