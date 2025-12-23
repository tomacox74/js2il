// Library module that exports a factory function and default instance

class Person {
    constructor(name, age) {
        this.name = name;
        this.age = age;
    }
    
    greet() {
        return "Hello, I am " + this.name + " and I am " + this.age + " years old.";
    }
}

// Export both a factory function and a default instance
module.exports = {
    createPerson: function(name, age) {
        return new Person(name, age);
    },
    defaultPerson: new Person("Default", 25)
};
