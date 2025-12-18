const p1 = Promise.resolve(1);
const p2 = Promise.reject("Error in p2");
const p3 = Promise.resolve(3);

Promise.allSettled([p1, p2, p3]).then((results) => {
    console.log("Result 0 status:", results[0].status, "value:", results[0].value);
    console.log("Result 1 status:", results[1].status, "reason:", results[1].reason);
    console.log("Result 2 status:", results[2].status, "value:", results[2].value);
});
