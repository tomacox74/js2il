"use strict";\r\n\r\nconst obj = {
    prefix: "Result: ",
    format: function (value) {
        return this.prefix + value;
    }
};

console.log(obj.format(42));
