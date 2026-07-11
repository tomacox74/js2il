// Early-bound reads and writes through generated accessors (phase 4, #1432).
const eligible = {
  text: "hello",
  num: 1,
  flag: true,
  fn: () => "fn"
};

console.log(eligible.text, eligible.num, eligible.flag, eligible.fn());

// Simple writes (num stays a stable double; text is demoted to object by the concat write).
eligible.num = 2;
eligible.num -= 1;
eligible.num *= 10;
console.log(eligible.num);

eligible.text = eligible.text + "!";
eligible.flag = !eligible.flag;
console.log(eligible.text, eligible.flag);

// Update expressions: postfix/prefix value semantics.
const counter = { n: 0 };
console.log(counter.n++);
console.log(++counter.n);
console.log(counter.n--);
console.log(--counter.n);
console.log(counter.n);

// Nullish-coalescing assignment on null / non-null members.
const nullable = { maybe: null, ready: "set" };
nullable.maybe ??= "filled";
nullable.ready ??= "ignored";
console.log(nullable.maybe, nullable.ready);

// Assignment expression values flow like the generic path.
const values = { a: 1, b: 2 };
const assigned = (values.a = 41) + 1;
const compounded = (values.b += 3);
console.log(assigned, values.a, compounded, values.b);

// Escaped literal stays on the generic path with identical behavior.
const escaped = { num: 10, text: "x" };
inspect(escaped);
escaped.num += 5;
escaped.num++;
console.log(escaped.num, JSON.stringify(escaped));

// Member used as a destructuring target must disqualify early binding.
const target = { val: 1 };
[target.val] = [42];
console.log(target.val);

const objTarget = { val: 1 };
({ v: objTarget.val } = { v: 43 });
console.log(objTarget.val);

function inspect(o) {
  console.log(Object.keys(o).join(","), o.num);
}
