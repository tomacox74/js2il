const p1 = Promise.resolve(1);
const p2 = Promise.resolve(2);
const p3 = Promise.resolve(3);

Promise.any([p1, p2, p3]).then((result) => {
    console.log("First resolved:", result);
});
