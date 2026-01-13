const FLAG = true;
const NAME = "typed";

function run() {
  // Force capture of FLAG and NAME from the outer (global) scope.
  return FLAG ? NAME : "untyped";
}

console.log(run());
