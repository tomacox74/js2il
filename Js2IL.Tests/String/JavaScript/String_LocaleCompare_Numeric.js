"use strict";

const values = ['2', '10', '1'];
values.sort((a, b) => a.localeCompare(b, 'en', { numeric: true }));
console.log(values.join(','));
