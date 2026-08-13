"use strict";

export default class Widget {
    constructor() {
        this.name = "widget";
    }

    describe() {
        return "a " + this.name;
    }
}
