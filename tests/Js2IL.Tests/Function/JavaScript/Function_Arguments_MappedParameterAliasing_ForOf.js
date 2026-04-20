var i = 0;

(function (a, b, c) {
  for (var value of arguments) {
    console.log(value);
    a = b;
    b = c;
    c = i;
    i++;
  }
}(1, 2, 3));

console.log(i);
