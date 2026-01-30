"use strict";\r\n\r\nvar counter = 0;
var message = "Initial message";

function changeGlobalValues() {
    console.log("Before change - counter:", counter, "message:", message);
    counter = counter + 10;
    message = "Modified message";
    console.log("After change - counter:", counter, "message:", message);
}

console.log("Start - counter:", counter, "message:", message);
changeGlobalValues();
console.log("End - counter:", counter, "message:", message);
