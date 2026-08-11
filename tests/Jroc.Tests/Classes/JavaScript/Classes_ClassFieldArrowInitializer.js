class Example {
    value = 4;
    read = () => this.value;
}

console.log(new Example().read());
