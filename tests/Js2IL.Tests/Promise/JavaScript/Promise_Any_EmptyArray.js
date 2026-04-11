"use strict";

Promise.any([])
    .then((result) => {
        console.log("Should not reach here");
    })
    .catch((error) => {
        console.log("Empty array rejected, error name:", error.name);
    });
