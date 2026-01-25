const stringObject = "abc";

try {
  stringObject.match();
} catch (e) {
  console.log(e.name);
  console.log(e.message);
}
