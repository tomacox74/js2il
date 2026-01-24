class Calculator {
    async add(a, b) {
        return await Promise.resolve(a + b);
    }
}

const calc = new Calculator();
calc.add(2, 3).then((result) => {
    console.log("Result:", result);
});
