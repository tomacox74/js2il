let missing;
let empty = null;
let zero = 0;
let no = false;

missing ??= "missing";
empty ??= "empty";
zero ??= 1;
no ??= true;

console.log(missing);
console.log(empty);
console.log(zero);
console.log(no);
