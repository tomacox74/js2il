const stringObject = "abc";

try {
  stringObject.doesNotExist();
} catch (e) {
  console.log(e.name);
  console.log(e.message);
}
