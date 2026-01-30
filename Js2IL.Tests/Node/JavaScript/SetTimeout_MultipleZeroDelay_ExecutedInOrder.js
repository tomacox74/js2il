"use strict";\r\n\r\nconst log1 = () =>{
    console.log("Hello 1st time");
}

const log2 = () =>{
    console.log("Hello 2nd time");
}

const log3 = () =>{
    console.log("Hello 3rd time");
}

setTimeout(log1, 0);
setTimeout(log2, 0);
setTimeout(log3, 0);
console.log("setTimeout with zero delay scheduled 3 times.");