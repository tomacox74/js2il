"use strict";

Promise.race([])
    .then((result) => {
        console.log("Resolved:", result);
    })
    .catch((error) => {
        console.log("Rejected:", error);
    });

// Empty race never settles, so we add a timeout to verify nothing happens
Promise.resolve("done").then(() => {
    console.log("Empty race is forever pending");
});
