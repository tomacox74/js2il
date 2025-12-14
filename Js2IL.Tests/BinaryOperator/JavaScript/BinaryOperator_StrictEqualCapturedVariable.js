// Test strict equality with captured variable in arrow function
// This reproduced a bug where Unknown type (boxed captured var) wasn't 
// being converted to number before comparison with numeric literal
// The key is that `id` is a parameter of the arrow function which gets
// stored in a scope class as an Object field with Unknown type.
function process(callback) {
    for (let i = 0; i < 10; i++) {
        callback(i);
    }
}

process((id) => {
    if (id === 3 || id === 7) {
        console.log("matched:", id);
    }
});
console.log("done");
