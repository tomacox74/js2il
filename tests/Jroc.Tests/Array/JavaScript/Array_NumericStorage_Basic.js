const values = [1, 2, 3, 4];
let total = 0;
for (let i = 0; i < values.length; i++) {
    total += values[i];
}

values[1] = 10;
const product = values[0] * values[3];
const numeric = +values[0];
console.log(total, values[1], values.length, product, numeric);

values[2] = "mixed";
values[4] = 5;
delete values[0];
console.log(values[2], values[4], Object.hasOwn(values, "0"), values.length);

const sized = new Array(4);
for (let i = 0; i < sized.length; i++) {
    sized[i] = i * 2;
}
console.log(sized.join(","), Object.keys(sized).length);
