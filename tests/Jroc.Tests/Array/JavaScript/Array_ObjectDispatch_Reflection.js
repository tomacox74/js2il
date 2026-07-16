const values = [];
values[10] = "ten";
values[2] = "two";
values.alpha = "named";
Object.defineProperty(values, "hidden", {
    value: "secret",
    enumerable: false,
    configurable: true
});
const marker = Symbol("marker");
values[marker] = "symbol";

function showKeys(keys) {
    return keys.map(function (key) {
        return typeof key === "symbol" ? "symbol" : key;
    }).join(",");
}

console.log(Object.keys(values).join(","));
console.log(Object.values(values).join(","));
console.log(Object.entries(values).map(function (entry) {
    return entry[0] + "=" + entry[1];
}).join(","));
console.log(Object.getOwnPropertyNames(values).join(","));
console.log(showKeys(Reflect.ownKeys(values)));
console.log(Object.keys(Object.getOwnPropertyDescriptors(values)).join(","));

const inherited = { 1: "inherited-index", inherited: "yes" };
Object.setPrototypeOf(values, inherited);
const forInKeys = [];
for (const key in values) {
    forInKeys.push(key);
}
console.log(forInKeys.join(","));
console.log("1" in values, "hidden" in values, "missing" in values);
Object.setPrototypeOf(values, Array.prototype);

console.log(delete values[2], delete values.alpha);
console.log(Object.getOwnPropertyNames(values).join(","));

const proxy = new Proxy(values, {});
console.log(Object.keys(proxy).join(","));
console.log(JSON.stringify(values));

const copied = { ...values };
console.log(Object.keys(copied).join(","));

try {
    Reflect.ownKeys("primitive");
} catch (error) {
    console.log(error.name);
}

const lengthGrowth = [];
Object.defineProperty(lengthGrowth, "length", { writable: true });
lengthGrowth.push("value");
console.log(lengthGrowth.length);
