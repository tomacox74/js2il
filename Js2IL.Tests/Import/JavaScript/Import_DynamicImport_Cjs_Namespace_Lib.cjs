"use strict";

let value = 1;

module.exports = {
    inc() {
        value = value + 1;
    },
    get value() {
        return value;
    }
};
