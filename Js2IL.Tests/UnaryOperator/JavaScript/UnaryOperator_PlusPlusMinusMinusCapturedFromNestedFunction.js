"use strict";\r\n\r\nfunction outer() {
    var x = 3;

    function inner() {
        console.log(x++);
        console.log(++x);
        console.log(x--);
        console.log(--x);
    }

    inner();
    console.log(x);
}

outer();
