const p1 = Promise.reject("Error first");
const p2 = Promise.resolve(2);
const p3 = Promise.resolve(3);

Promise.race([p1, p2, p3])
    .then((result) => {
        console.log("Should not reach here");
    })
    .catch((error) => {
        console.log("First was rejection:", error);
    });
