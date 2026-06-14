(function (a, b, c) {
    let i = 0;

    for (var value of arguments) {
        a = b;
        b = c;
        c = i;
        console.log(value);
        i++;
    }

    console.log(i);
}(1, 2, 3));
