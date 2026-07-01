const f = function g(n) {
    if (n === 0) {
        console.log(g === f);
        return 1;
    }

    return g(n - 1) + 1;
};

console.log(f(2));
