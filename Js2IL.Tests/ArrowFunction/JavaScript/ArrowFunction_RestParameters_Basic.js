"use strict";

const sum = (...numbers) => {
    let total = 0;
    for (let i = 0; i < numbers.length; i++) {
        total = total + numbers[i];
    }
    return total;
};

console.log(sum(1, 2, 3));
console.log(sum(10, 20));
console.log(sum());
