"use strict";\r\n\r\nconst globalVar = "global";

const testFunction = () => {
    const outerVar = "outer";

    const nestedFunction = () => {
        const innerVar = "inner";
        console.log("Global:", globalVar);
        console.log("Outer:", outerVar);
        console.log("Inner:", innerVar);
    };

    nestedFunction();
};

testFunction();
