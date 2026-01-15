// Test await inside try/catch - rejection should be caught
async function testTryCatch() {
    console.log("Before try");
    try {
        console.log("Inside try, before await");
        const result = await Promise.reject("error value");
        console.log("This should not print");
    } catch (e) {
        console.log("Caught error:", e);
    }
    console.log("After try/catch");
}

testTryCatch().then(() => {
    console.log("Async function completed");
});

console.log("After calling async function");
