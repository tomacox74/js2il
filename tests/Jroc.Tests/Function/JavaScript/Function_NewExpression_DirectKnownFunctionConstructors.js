function DeclaredPoint(x, y) {
    this.sum = x + y;
}

const ExpressionPoint = function (x, y) {
    this.sum = x * y;
};

console.log(new DeclaredPoint(2, 3).sum);
console.log(new ExpressionPoint(4, 5).sum);
