assertRelativeDateMs(new Date(1899, 11, 31), -2209075200000);

assertRelativeDateMs(new Date(1899, 12, 1), -2208988800000);

assertRelativeDateMs(new Date(1900, 0, 1), -2208988800000);

assertRelativeDateMs(new Date(1969, 11, 31), -86400000);

assertRelativeDateMs(new Date(1969, 12, 1), 0);

assertRelativeDateMs(new Date(1970, 0, 1), 0);

assertRelativeDateMs(new Date(1999, 11, 31), 946598400000);

assertRelativeDateMs(new Date(1999, 12, 1), 946684800000);

assertRelativeDateMs(new Date(2000, 0, 1), 946684800000);

assertRelativeDateMs(new Date(2099, 11, 31), 4102358400000);

assertRelativeDateMs(new Date(2099, 12, 1), 4102444800000);

assertRelativeDateMs(new Date(2100, 0, 1), 4102444800000);
