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

// Segment records are ordinary mutable objects: delete removes the key and a
// later re-add appends it in encounter order.
const record = new Intl.Segmenter().segment("x")[Symbol.iterator]().next().value;
console.log(Object.keys(record).join(","));
console.log(delete record.segment);
console.log(record.segment, "segment" in record, Object.keys(record).join(","));
record.segment = "y";
console.log(record.segment, Object.keys(record).join(","));

// %SegmentIteratorPrototype% carries only next + @@toStringTag and inherits @@iterator.
const iteratorPrototype = Object.getPrototypeOf(iterator);
console.log(Object.prototype.toString.call(iterator));
console.log(Object.getOwnPropertySymbols(iteratorPrototype).map(String).join(","));
console.log(iterator[Symbol.iterator]() === iterator);
