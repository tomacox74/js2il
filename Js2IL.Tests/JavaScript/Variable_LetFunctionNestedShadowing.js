let a = 0;
function outer() {
  let a = 1;
  function inner() {
    let a = 2;
    console.log(a);
  }
  inner();
  console.log(a);
}
outer();
console.log(a);
