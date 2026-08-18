"use strict";

const p1 = Promise.resolve(1);
Promise.prototype.then.call(p1, value => {
    console.log(`then.call value: ${value}`);
});

const p2 = Promise.reject(new Error("boom"));
Promise.prototype.catch.call(p2, error => {
    console.log(`catch.call message: ${error.message}`);
});

let finallyRan = false;
const p3 = Promise.resolve(7);
Promise.prototype.finally.call(p3, () => {
    finallyRan = true;
}).then(value => {
    console.log(`finally.call ran: ${finallyRan}, chained value: ${value}`);
});

// Receiver errors.
try {
    Promise.prototype.then.call({});
} catch (error) {
    console.log(error instanceof TypeError);
}
try {
    Promise.prototype.catch.call({});
} catch (error) {
    console.log(error instanceof TypeError);
}
try {
    Promise.prototype.finally.call({});
} catch (error) {
    console.log(error instanceof TypeError);
}

console.log(Promise.prototype.then.length);
console.log(Promise.prototype.catch.length);
console.log(Promise.prototype.finally.length);
