'use strict';

function makeCounter(start) {
    let count = start;
    return function increment() {
        count = count + 1;
        return count;
    };
}

const counter = makeCounter(0);
console.log('count=' + counter());
console.log('count=' + counter());
console.log('count=' + counter());
console.log('CANARY:closures:ok');
