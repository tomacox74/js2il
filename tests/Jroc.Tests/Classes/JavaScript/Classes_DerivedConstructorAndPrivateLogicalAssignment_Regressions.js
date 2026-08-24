class Base {
  constructor(value) {
    this.value = value;
    this.argumentCount = arguments.length;
    this.newTargetName = new.target.name;
  }
}

class ExplicitDerived extends Base {
  constructor(value) {
    super(value);
  }
}

class ImplicitDerived extends Base {}

for (const Constructor of [ExplicitDerived, ImplicitDerived]) {
  const instance = new Constructor(42);
  console.log(instance.value);
  console.log(instance.argumentCount);
  console.log(instance.newTargetName);
}

class AccessorBox {
  #value = 0;

  get #slot() {
    return this.#value;
  }

  set #slot(value) {
    this.#value = value;
  }

  assign(other) {
    return other.#slot ||= 5;
  }

  value() {
    return this.#value;
  }
}

const accessorSource = new AccessorBox();
const accessorTarget = new AccessorBox();
console.log(accessorSource.assign(accessorTarget));
console.log(accessorSource.value());
console.log(accessorTarget.value());

class FieldBox {
  #value = 0;

  assign(getReceiver) {
    return getReceiver().#value ||= 5;
  }

  value() {
    return this.#value;
  }
}

const fieldBox = new FieldBox();
let receiverEvaluationCount = 0;
console.log(fieldBox.assign(() => {
  receiverEvaluationCount++;
  return fieldBox;
}));
console.log(receiverEvaluationCount);
console.log(fieldBox.value());
