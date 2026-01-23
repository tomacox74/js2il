// Test: Import a function exported from another module and call it
// This is the main entry point that imports and uses the function

const greet = require('./CommonJS_Export_Function_Lib');

// Call the imported function
const result = greet("World");
console.log("result:", result);
