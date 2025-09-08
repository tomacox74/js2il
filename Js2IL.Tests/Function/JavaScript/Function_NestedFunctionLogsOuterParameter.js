function outerFunction(outerParam) {
    function innerFunction(innerParam) {
        console.log("Outer Param:", outerParam);
        console.log("Inner Param:", innerParam);
    }
    return innerFunction;
}

var inner = outerFunction("Value from outer");
inner("Value from inner");
