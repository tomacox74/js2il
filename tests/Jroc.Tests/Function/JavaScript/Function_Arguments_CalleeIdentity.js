function inspect() {
    console.log(arguments.callee === inspect);
    console.log(typeof arguments.callee);
}

function spreadThis() {
    console.log(this === globalThis);
}

function outer() {
    console.log(arguments.callee === outer);
    inspect(...[1, 2, 3]);
    console.log(arguments.callee === outer);
}

inspect(1);
spreadThis(...[]);
outer();
