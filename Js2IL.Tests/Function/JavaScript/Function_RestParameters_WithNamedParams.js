"use strict";

function combine(separator, ...items) {
    let result = "";
    for (let i = 0; i < items.length; i++) {
        if (i > 0) {
            result = result + separator;
        }
        result = result + items[i];
    }
    return result;
}

console.log(combine("-", "a", "b", "c"));
console.log(combine(" ", "hello", "world"));
console.log(combine(","));
