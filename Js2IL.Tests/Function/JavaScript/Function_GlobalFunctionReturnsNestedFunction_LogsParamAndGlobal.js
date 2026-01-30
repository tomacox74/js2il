"use strict";\r\n\r\nvar outerVar = 10;

function createFunction(param) {
    function nestedFunction() {
        var innerVar = 5;
        console.log("Outer variable:", outerVar);
        console.log("Parameter:", param);
        console.log("Inner variable:", innerVar);
        return param + innerVar;
    }
    return nestedFunction;
}

var resultFunc = createFunction(7);
var result = resultFunc();
console.log("Result:", result);
