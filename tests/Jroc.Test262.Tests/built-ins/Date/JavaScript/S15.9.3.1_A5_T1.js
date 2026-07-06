assertRelativeDateMs(new Date(1899, 11), -2211667200000);

assertRelativeDateMs(new Date(1899, 12), -2208988800000);

assertRelativeDateMs(new Date(1900, 0), -2208988800000);

assertRelativeDateMs(new Date(1969, 11), -2678400000);

assertRelativeDateMs(new Date(1969, 12), 0);

assertRelativeDateMs(new Date(1970, 0), 0);

assertRelativeDateMs(new Date(1999, 11), 944006400000);

assertRelativeDateMs(new Date(1999, 12), 946684800000);

assertRelativeDateMs(new Date(2000, 0), 946684800000);

assertRelativeDateMs(new Date(2099, 11), 4099766400000);

assertRelativeDateMs(new Date(2099, 12), 4102444800000);

assertRelativeDateMs(new Date(2100, 0), 4102444800000);
