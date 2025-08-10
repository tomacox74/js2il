var g = "Hello";

function outer(p) {
    function inner() {
        console.log("param:", p);
        console.log("global:", g);
    }
    return inner;
}

var f = outer(123);
f();
