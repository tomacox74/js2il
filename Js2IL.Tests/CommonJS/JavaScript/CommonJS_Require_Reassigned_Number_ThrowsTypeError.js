"use strict";

require = 1;
try {
    require('fs');
} catch (e) {
    console.log(e.name);
    console.log(e.message);
}
