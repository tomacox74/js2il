const arr = [0,1,2,3,4,5];
console.log(arr.slice().join(','));
console.log(arr.slice(2).join(','));
console.log(arr.slice(2, 4).join(','));
console.log(arr.slice(-2).join(','));
console.log(arr.slice(1, -1).join(','));
console.log(arr.slice(10).join(','));
console.log(arr.slice(-10, 2).join(','));
