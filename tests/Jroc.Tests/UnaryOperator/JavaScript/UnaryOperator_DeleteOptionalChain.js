let getterCallCount = 0;
const object = {
  get value() {
    getterCallCount++;
    return 1;
  }
};

console.log(delete object?.value);
console.log("value" in object);
console.log(getterCallCount);

let indexEvaluationCount = 0;
const nullValue = null;
console.log(delete nullValue?.[indexEvaluationCount++]);
console.log(indexEvaluationCount);

const indexed = { key: 1 };
console.log(delete indexed?.[(() => {
  indexEvaluationCount++;
  return "key";
})()]);
console.log(indexEvaluationCount);
console.log("key" in indexed);
