"use strict";\r\n\r\nconst logMessage = (name, age, city) => {
    console.log("Name: " + name);
    console.log("Age: " + age);
    console.log("City: " + city);
}

setTimeout(logMessage, 0, "Alice", 30, "Seattle");
console.log("setTimeout with zero delay and arguments scheduled.");
