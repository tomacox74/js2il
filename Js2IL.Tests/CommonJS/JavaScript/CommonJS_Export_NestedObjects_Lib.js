"use strict";

// Library module that exports nested literal objects with fields and methods

module.exports = {
    name: "Calculator",
    version: 1,
    math: {
        add: function(a, b) {
            return a + b;
        },
        multiply: function(a, b) {
            return a * b;
        }
    },
    utils: {
        prefix: "Result: ",
        formatNum: function(value) {
            return "Num: " + value;
        }
    }
};
