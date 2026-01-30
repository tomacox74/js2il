"use strict";

Promise.allSettled(null)
    .then((result) => {
        console.log("Should not reach here");
    })
    .catch((error) => {
        console.log("Caught error type:", error.name);
    });
