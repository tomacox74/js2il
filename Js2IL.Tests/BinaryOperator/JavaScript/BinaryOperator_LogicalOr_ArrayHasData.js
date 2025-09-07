function arrayHasData(arr) {
    if (!arr || !arr.length) {
        return false;
    }
    return true;
}

console.log(arrayHasData(undefined));
console.log(arrayHasData([]));
console.log(arrayHasData([1,2,3]));
