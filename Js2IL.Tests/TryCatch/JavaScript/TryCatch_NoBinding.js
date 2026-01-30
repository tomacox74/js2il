"use strict";\r\n\r\ntry {
    console.log("before throw");
    throw new Error("test error message");
} catch {
    console.log("in catch");
}

console.log("after catch");
