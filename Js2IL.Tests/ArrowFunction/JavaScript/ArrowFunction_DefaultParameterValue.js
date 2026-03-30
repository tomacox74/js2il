"use strict";

// Test default parameter values in arrow functions
const greet = (name = "World") => {
    console.log("Hello, " + name);
};

const add = (a, b = 10) => {
    console.log(a + b);
};

const multi = (x = 5, y = 3, z = 2) => {
    console.log(x * y * z);
};

// Test with no arguments (all defaults)
greet();

// Test with argument provided (no default)
greet("Alice");

// Test with partial arguments
add(5);
add(5, 15);

// Test with multiple defaults
multi();
multi(2);
multi(2, 4);
multi(2, 4, 3);
