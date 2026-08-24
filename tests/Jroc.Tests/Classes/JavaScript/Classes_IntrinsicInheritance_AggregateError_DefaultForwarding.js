"use strict";

class ExplicitEmpty extends AggregateError {
    constructor(errors) {
        super();
    }
}

try {
    new ExplicitEmpty([]);
    console.log(false);
} catch (error) {
    console.log(error instanceof TypeError);
}

class ImplicitDefault extends AggregateError {}

const error = new ImplicitDefault([1, 2]);
console.log(Error.isError(error));
console.log(error.errors.length);
