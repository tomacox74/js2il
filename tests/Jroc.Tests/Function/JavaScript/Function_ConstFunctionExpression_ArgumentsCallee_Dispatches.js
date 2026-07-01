const f = function () {
    return arguments.callee === f;
};

console.log(f());
