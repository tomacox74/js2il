"use strict";\r\n\r\nPromise.any(null)
    .then((result) => {
        console.log("Should not reach here");
    })
    .catch((error) => {
        console.log("Caught error type:", error.name);
    });
