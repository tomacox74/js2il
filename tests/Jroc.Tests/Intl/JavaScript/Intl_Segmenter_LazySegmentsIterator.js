const input = "A😀B";
const segments = new Intl.Segmenter().segment(input);
const iterator = segments[Symbol.iterator]();

console.log(Object.getPrototypeOf(segments) !== null);
console.log(iterator !== segments);
console.log(segments[Symbol.iterator]() !== iterator);
console.log(iterator.return);

const first = iterator.next();
console.log(first.done);
console.log(first.value.segment);
console.log(first.value.index);
console.log(first.value.input === input);

const second = iterator.next();
console.log(second.value.segment);
console.log(second.value.index);

const third = iterator.next();
console.log(third.value.segment);
console.log(third.value.index);

console.log(iterator.next().done);
console.log(iterator.next().done);
