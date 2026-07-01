function Point(x) {
    this.value = x + 1;
}

for ({ Point } of [{
    Point: function (x) {
        this.value = x + 20;
    }
}]) {
}

console.log(new Point(5).value);
