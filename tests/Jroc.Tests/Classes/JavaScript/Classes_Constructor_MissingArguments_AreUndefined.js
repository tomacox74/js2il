"use strict";

class Example {
    constructor(first, second, third) {
        console.log(first);
        console.log(second === undefined);
        console.log(third === undefined);
    }
}

new Example("provided");
