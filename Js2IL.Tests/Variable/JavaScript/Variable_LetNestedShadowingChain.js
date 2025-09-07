let v = 0;
{
  let v = 1;
  console.log(v);
  {
    let v = 2;
    console.log(v);
    {
      let v = 3;
      console.log(v);
    }
    console.log(v);
  }
  console.log(v);
}
console.log(v);
