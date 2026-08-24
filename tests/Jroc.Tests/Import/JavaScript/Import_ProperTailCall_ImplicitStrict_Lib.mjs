function recurse(n) {
    if (n === 0) {
        return 42;
    }

    return recurse(n - 1);
}

export const result = recurse(100000);
