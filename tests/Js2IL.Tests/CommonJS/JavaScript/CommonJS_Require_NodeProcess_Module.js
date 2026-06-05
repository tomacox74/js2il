const processModule = require("node:process");

console.log(processModule === process);
console.log(typeof processModule.env);
