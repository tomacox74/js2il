function outerFunction() {
    console.log("Before nested function declaration");
    
    function innerFunction() {
        console.log("Inside nested function");
        return "nested result";
    }
    
    console.log("After nested function declaration");
    var result = innerFunction();
    console.log("Nested function returned:", result);
    
    return "outer result";
}

console.log("Start");
var mainResult = outerFunction();
console.log("Main result:", mainResult);
console.log("End");
