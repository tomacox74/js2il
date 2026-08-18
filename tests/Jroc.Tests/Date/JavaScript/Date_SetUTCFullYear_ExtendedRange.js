const date = new Date("+275760-09-12T23:59:59.999Z");
const result = date.setUTCFullYear(275760);

console.log(Number.isNaN(result));
console.log(date.getUTCFullYear());
console.log(date.getUTCMonth());
console.log(date.getUTCDate());
