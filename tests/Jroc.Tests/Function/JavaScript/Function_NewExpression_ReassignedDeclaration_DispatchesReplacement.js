function Point(x) {
    this.value = x + 1;
}

Point = function (x) {
    this.value = x + 10;
};

console.log(new Point(5).value);
