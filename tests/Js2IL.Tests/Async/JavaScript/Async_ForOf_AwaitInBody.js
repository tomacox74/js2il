async function sum(values) {
    let total = 0;
    for (const value of values) {
        await Promise.resolve();
        total += value;
    }
    return total;
}

console.log(await sum([1, 2, 3]));
