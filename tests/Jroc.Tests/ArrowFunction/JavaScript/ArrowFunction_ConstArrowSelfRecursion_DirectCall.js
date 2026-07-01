const values = [];

const loop = (n) => {
    if (n <= 0) {
        return 0;
    }

    values.push(n);
    return loop(n - 1) + n;
};

console.log(loop(3));
console.log(values.join(","));
