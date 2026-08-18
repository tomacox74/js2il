try {
  String(Object.create(null));
} catch (error) {
  console.log(error.name);
}
