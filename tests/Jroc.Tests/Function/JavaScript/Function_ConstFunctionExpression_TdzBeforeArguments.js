function arg() {
    console.log("arg");
    return 1;
}

try {
    f(arg());
} catch (error) {
    console.log(error.name);
}

const f = function () {
    console.log("called");
};
