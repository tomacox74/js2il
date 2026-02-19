"use strict";

const util = require('util');

// Test inspect with objects
const obj = {
    name: 'John',
    age: 30,
    active: true
};

console.log(util.inspect(obj));

// Nested object
const nested = {
    person: {
        name: 'Jane',
        age: 25
    },
    city: 'NYC'
};

console.log(util.inspect(nested));
