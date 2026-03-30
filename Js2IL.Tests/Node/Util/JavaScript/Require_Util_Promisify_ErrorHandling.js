"use strict";

const util = require('util');

// Create a callback-style function that errors
function callbackFn(shouldError, callback) {
    if (shouldError) {
        callback(new Error('Test error'));
    } else {
        callback(null, 'success');
    }
}

// Promisify the function
const promisified = util.promisify(callbackFn);

// Test success case
promisified(false).then(result => {
    console.log('Success:', result);
}).catch(err => {
    console.log('Error:', err.message);
});

// Test error case
promisified(true).then(result => {
    console.log('Success:', result);
}).catch(err => {
    console.log('Error:', err.message);
});
