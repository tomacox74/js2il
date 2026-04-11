"use strict";

const util = require('util');

// Create a callback-style function
function callbackFn(value, callback) {
    // Success case: error is null, result is value * 2
    callback(null, value * 2);
}

// Promisify the function
const promisified = util.promisify(callbackFn);

// Use it with promises
promisified(5).then(result => {
    console.log(result); // Should print 10
});
