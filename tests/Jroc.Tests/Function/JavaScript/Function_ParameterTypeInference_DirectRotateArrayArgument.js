"use strict";

function RotateX(M, Phi) {
    const R = [[1, 0], [0, Phi]];
    return M[0][0] + R[1][1];
}

function RotateY(M, Phi) {
    const R = [[Phi, 0], [0, 1]];
    return M[0][0] + R[0][0];
}

function RotateZ(M, Phi) {
    const R = [[1, Phi], [0, 1]];
    return M[0][0] + R[0][1];
}

const matrix = [[1, 0], [0, 1]];

console.log(RotateX(matrix, 30));
console.log(RotateY(matrix, 45));
console.log(RotateZ(matrix, 60));
