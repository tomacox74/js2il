const p1 = Promise.reject("Error 1");
const p2 = Promise.reject("Error 2");
const p3 = Promise.reject("Error 3");

Promise.any([p1, p2, p3])
    .then((result) => {
        console.log("Should not reach here");
    })
    .catch((error) => {
        console.log("All rejected, error name:", error.name);
        console.log("Errors count:", error.errors.length);
    });
