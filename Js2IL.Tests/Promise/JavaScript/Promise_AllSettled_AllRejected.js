const p1 = Promise.reject("Error 1");
const p2 = Promise.reject("Error 2");

Promise.allSettled([p1, p2]).then((results) => {
    console.log("Result 0 status:", results[0].status, "reason:", results[0].reason);
    console.log("Result 1 status:", results[1].status, "reason:", results[1].reason);
});
