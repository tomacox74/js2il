const logHello = () =>{
    console.log("Hello, World!  One second has elapsed.");
}

// note.. mock should be in place to simulate the passage of time
setTimeout(logHello,1000);
console.log("setTimeout with 1 second delay scheduled.");