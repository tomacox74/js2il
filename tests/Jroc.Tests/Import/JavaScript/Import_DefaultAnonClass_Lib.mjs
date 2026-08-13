"use strict";

export default class {
    constructor() {
        this.kind = "anon-class";
    }

    greet() {
        return "hello from " + this.kind;
    }
}
