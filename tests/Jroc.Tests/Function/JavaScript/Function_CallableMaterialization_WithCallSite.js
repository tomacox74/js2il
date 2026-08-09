const target = () => "lexical";

with ({ target: () => "with" }) {
    console.log(target());
}

console.log(target());
