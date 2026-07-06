assertRelativeDateMs(new Date(1899, 11, 31, 23, 59, 59), -2208988801000);

assertRelativeDateMs(new Date(1899, 12, 1, 0, 0, 0), -2208988800000);

assertRelativeDateMs(new Date(1900, 0, 1, 0, 0, 0), -2208988800000);

assertRelativeDateMs(new Date(1969, 11, 31, 23, 59, 59), -1000);

assertRelativeDateMs(new Date(1969, 12, 1, 0, 0, 0), 0);

assertRelativeDateMs(new Date(1970, 0, 1, 0, 0, 0), 0);

assertRelativeDateMs(new Date(1999, 11, 31, 23, 59, 59), 946684799000);

assertRelativeDateMs(new Date(1999, 12, 1, 0, 0, 0), 946684800000);

assertRelativeDateMs(new Date(2000, 0, 1, 0, 0, 0), 946684800000);

assertRelativeDateMs(new Date(2099, 11, 31, 23, 59, 59), 4102444799000);

assertRelativeDateMs(new Date(2099, 12, 1, 0, 0, 0), 4102444800000);

assertRelativeDateMs(new Date(2100, 0, 1, 0, 0, 0), 4102444800000);
