"use strict";

let Function = class {
    constructor(source) {
        this.source = source;
    }
};

let instance = new Function("if (");
console.log(instance.source);
