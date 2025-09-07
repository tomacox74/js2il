var globalVar = "I am global";

function testFunction() {
    var outerVar = "I am outer";

    function innerFunction() {
        var innerVar = "I am inner";
        console.log("Global:", globalVar);
        console.log("Outer:", outerVar);
        console.log("Inner:", innerVar);
    }

    innerFunction();
}

testFunction();
