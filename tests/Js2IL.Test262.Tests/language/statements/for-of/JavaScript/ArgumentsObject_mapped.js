(function () {
    let i = 0;

    for (var value of arguments) {
        console.log(Object.is(value, arguments[i]));
        i++;
    }

    console.log(i);
}(0, "a", true, false, null, undefined, NaN));
