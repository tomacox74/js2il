setTimeout(() => console.log("timeout"), 0);
setImmediate(() => console.log("immediate"));
console.log("scheduled immediate and timeout.");
