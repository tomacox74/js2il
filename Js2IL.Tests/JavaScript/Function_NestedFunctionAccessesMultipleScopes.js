function testFunction() {
    var outerVar = "outer";
    
    function nestedFunction() {
        var innerVar = "inner";
        console.log("Outer:", outerVar);
        console.log("Inner:", innerVar);
    }
    
    nestedFunction();
}

testFunction();
