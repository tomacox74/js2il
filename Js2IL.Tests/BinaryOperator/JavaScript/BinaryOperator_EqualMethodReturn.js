"use strict";\r\n\r\nclass TestClass {
    getValue() {
        return 4;
    }
}

let obj = new TestClass();
let methodResult = obj.getValue();
let literalValue = 4;

console.log("methodResult == literalValue:", methodResult == literalValue);
console.log("literalValue == methodResult:", literalValue == methodResult);
console.log("methodResult === literalValue:", methodResult === literalValue);
