class Greeter {
	constructor(name) {
		this.name = name;
	}
	sayName() {
		console.log(this.name);
	}
}

const g = new Greeter("Alice");
g.sayName();
