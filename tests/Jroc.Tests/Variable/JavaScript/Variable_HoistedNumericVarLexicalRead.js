function c1() { let y = z; var z = 1; return y; }
function c2() { const y = z; var z = 1; return y; }
function c3() { let y = z; var z = 1; return typeof y; }
function c4() { let y; y = z; var z = 1; return y; }
function c5() { if (true) { let y = z; var z = 1; return y; } }
function c6() { var z = 1; let y = w; var w = 2; return y; }
function initializedAssignment() { let y = 2; y = z; var z = 1; return y; }
function alias() { let y = z; const result = y; var z = 1; return result; }
function nestedAlias() { let y = z; var z = 1; { const result = y; return result; } }
function conditional() { let y = z; var z = 1; return true ? y : 0; }
function ok1() { var y = z; var z = 1; return y; }
function ok2() { let y = z; var z = "s"; return y; }
function ok3() { var z; let y = z; z = 1; return y; }

console.log([c1(), c2(), c3(), c4(), c5(), c6(), initializedAssignment(), alias(), nestedAlias(), conditional(), ok1(), ok2(), ok3()]
    .map(String).join(" | "));

function initializedLet() { var z = 1; let y = z; return y; }
function initializedConst() { var z = 2; const y = z; return y; }
function initializedWrite() { let y; var z = 3; y = z; return y; }
console.log(initializedLet(), initializedConst(), initializedWrite());
