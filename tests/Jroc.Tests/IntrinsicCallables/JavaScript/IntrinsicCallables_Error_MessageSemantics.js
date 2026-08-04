"use strict";

console.log(new Error(undefined).message === new Error().message);
console.log(Error(undefined).message === Error().message);

try {
    console.log(new Error(Symbol("message")).message);
} catch (error) {
    console.log(error.name);
}

try {
    console.log(TypeError(Symbol("message")).message);
} catch (error) {
    console.log(error.name);
}
