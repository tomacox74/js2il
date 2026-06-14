try {
    await Promise.reject("boom");
} catch (error) {
    console.log("caught:", error);
}

console.log("after");
