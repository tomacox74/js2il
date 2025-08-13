let v = 0;
{
  let v = 1;
  console.log(v); // 1
  {
    let v = 2;
    console.log(v); // 2
    {
      let v = 3;
      console.log(v); // 3
    }
    console.log(v); // 2
  }
  console.log(v); // 1
}
console.log(v); // 0
