<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 21.4: Date Objects

[Back to Section21](Section21.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 21.4 | Date Objects | Supported | [tc39.es](https://tc39.es/ecma262/#sec-date-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 21.4.1 | Overview of Date Objects and Definitions of Abstract Operations | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-overview-of-date-objects-and-definitions-of-abstract-operations) |
| 21.4.1.1 | Time Values and Time Range | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-time-values-and-time-range) |
| 21.4.1.2 | Time-related Constants | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-time-related-constants) |
| 21.4.1.3 | Day ( t ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-day) |
| 21.4.1.4 | TimeWithinDay ( t ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-timewithinday) |
| 21.4.1.5 | DaysInYear ( y ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-daysinyear) |
| 21.4.1.6 | DayFromYear ( y ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dayfromyear) |
| 21.4.1.7 | TimeFromYear ( y ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-timefromyear) |
| 21.4.1.8 | YearFromTime ( t ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-yearfromtime) |
| 21.4.1.9 | DayWithinYear ( t ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-daywithinyear) |
| 21.4.1.10 | InLeapYear ( t ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-inleapyear) |
| 21.4.1.11 | MonthFromTime ( t ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-monthfromtime) |
| 21.4.1.12 | DateFromTime ( t ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-datefromtime) |
| 21.4.1.13 | WeekDay ( t ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-weekday) |
| 21.4.1.14 | HourFromTime ( t ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-hourfromtime) |
| 21.4.1.15 | MinFromTime ( t ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-minfromtime) |
| 21.4.1.16 | SecFromTime ( t ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-secfromtime) |
| 21.4.1.17 | msFromTime ( t ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-msfromtime) |
| 21.4.1.18 | GetUTCEpochNanoseconds ( year , month , day , hour , minute , second , millisecond , microsecond , nanosecond ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getutcepochnanoseconds) |
| 21.4.1.19 | Time Zone Identifiers | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-time-zone-identifiers) |
| 21.4.1.20 | GetNamedTimeZoneEpochNanoseconds ( timeZoneIdentifier , year , month , day , hour , minute , second , millisecond , microsecond , nanosecond ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getnamedtimezoneepochnanoseconds) |
| 21.4.1.21 | GetNamedTimeZoneOffsetNanoseconds ( timeZoneIdentifier , epochNanoseconds ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getnamedtimezoneoffsetnanoseconds) |
| 21.4.1.22 | Time Zone Identifier Record | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-time-zone-identifier-record) |
| 21.4.1.23 | AvailableNamedTimeZoneIdentifiers ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-availablenamedtimezoneidentifiers) |
| 21.4.1.24 | SystemTimeZoneIdentifier ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-systemtimezoneidentifier) |
| 21.4.1.25 | LocalTime ( t ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-localtime) |
| 21.4.1.26 | UTC ( t ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-utc-t) |
| 21.4.1.27 | MakeTime ( hour , min , sec , ms ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-maketime) |
| 21.4.1.28 | MakeDay ( year , month , date ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-makeday) |
| 21.4.1.29 | MakeDate ( day , time ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-makedate) |
| 21.4.1.30 | MakeFullYear ( year ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-makefullyear) |
| 21.4.1.31 | TimeClip ( time ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-timeclip) |
| 21.4.1.32 | Date Time String Format | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date-time-string-format) |
| 21.4.1.32.1 | Expanded Years | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-expanded-years) |
| 21.4.1.33 | Time Zone Offset String Format | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-time-zone-offset-strings) |
| 21.4.1.33.1 | IsTimeZoneOffsetString ( offsetString ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-istimezoneoffsetstring) |
| 21.4.1.33.2 | ParseTimeZoneOffsetString ( offsetString ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-parsetimezoneoffsetstring) |
| 21.4.2 | The Date Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date-constructor) |
| 21.4.2.1 | Date ( ... values ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date) |
| 21.4.3 | Properties of the Date Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-date-constructor) |
| 21.4.3.1 | Date.now ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.now) |
| 21.4.3.2 | Date.parse ( string ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.parse) |
| 21.4.3.3 | Date.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype) |
| 21.4.3.4 | Date.UTC ( year [ , month [ , date [ , hours [ , minutes [ , seconds [ , ms ] ] ] ] ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.utc) |
| 21.4.4 | Properties of the Date Prototype Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-date-prototype-object) |
| 21.4.4.1 | Date.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.constructor) |
| 21.4.4.2 | Date.prototype.getDate ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getdate) |
| 21.4.4.3 | Date.prototype.getDay ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getday) |
| 21.4.4.4 | Date.prototype.getFullYear ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getfullyear) |
| 21.4.4.5 | Date.prototype.getHours ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.gethours) |
| 21.4.4.6 | Date.prototype.getMilliseconds ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getmilliseconds) |
| 21.4.4.7 | Date.prototype.getMinutes ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getminutes) |
| 21.4.4.8 | Date.prototype.getMonth ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getmonth) |
| 21.4.4.9 | Date.prototype.getSeconds ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getseconds) |
| 21.4.4.10 | Date.prototype.getTime ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.gettime) |
| 21.4.4.11 | Date.prototype.getTimezoneOffset ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.gettimezoneoffset) |
| 21.4.4.12 | Date.prototype.getUTCDate ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutcdate) |
| 21.4.4.13 | Date.prototype.getUTCDay ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutcday) |
| 21.4.4.14 | Date.prototype.getUTCFullYear ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutcfullyear) |
| 21.4.4.15 | Date.prototype.getUTCHours ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutchours) |
| 21.4.4.16 | Date.prototype.getUTCMilliseconds ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutcmilliseconds) |
| 21.4.4.17 | Date.prototype.getUTCMinutes ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutcminutes) |
| 21.4.4.18 | Date.prototype.getUTCMonth ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutcmonth) |
| 21.4.4.19 | Date.prototype.getUTCSeconds ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutcseconds) |
| 21.4.4.20 | Date.prototype.setDate ( date ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setdate) |
| 21.4.4.21 | Date.prototype.setFullYear ( year [ , month [ , date ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setfullyear) |
| 21.4.4.22 | Date.prototype.setHours ( hour [ , min [ , sec [ , ms ] ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.sethours) |
| 21.4.4.23 | Date.prototype.setMilliseconds ( ms ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setmilliseconds) |
| 21.4.4.24 | Date.prototype.setMinutes ( min [ , sec [ , ms ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setminutes) |
| 21.4.4.25 | Date.prototype.setMonth ( month [ , date ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setmonth) |
| 21.4.4.26 | Date.prototype.setSeconds ( sec [ , ms ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setseconds) |
| 21.4.4.27 | Date.prototype.setTime ( time ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.settime) |
| 21.4.4.28 | Date.prototype.setUTCDate ( date ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setutcdate) |
| 21.4.4.29 | Date.prototype.setUTCFullYear ( year [ , month [ , date ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setutcfullyear) |
| 21.4.4.30 | Date.prototype.setUTCHours ( hour [ , min [ , sec [ , ms ] ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setutchours) |
| 21.4.4.31 | Date.prototype.setUTCMilliseconds ( ms ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setutcmilliseconds) |
| 21.4.4.32 | Date.prototype.setUTCMinutes ( min [ , sec [ , ms ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setutcminutes) |
| 21.4.4.33 | Date.prototype.setUTCMonth ( month [ , date ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setutcmonth) |
| 21.4.4.34 | Date.prototype.setUTCSeconds ( sec [ , ms ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setutcseconds) |
| 21.4.4.35 | Date.prototype.toDateString ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.todatestring) |
| 21.4.4.36 | Date.prototype.toISOString ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.toisostring) |
| 21.4.4.37 | Date.prototype.toJSON ( key ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.tojson) |
| 21.4.4.38 | Date.prototype.toLocaleDateString ( [ reserved1 [ , reserved2 ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.tolocaledatestring) |
| 21.4.4.39 | Date.prototype.toLocaleString ( [ reserved1 [ , reserved2 ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.tolocalestring) |
| 21.4.4.40 | Date.prototype.toLocaleTimeString ( [ reserved1 [ , reserved2 ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.tolocaletimestring) |
| 21.4.4.41 | Date.prototype.toString ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.tostring) |
| 21.4.4.41.1 | TimeString ( tv ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-timestring) |
| 21.4.4.41.2 | DateString ( tv ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-datestring) |
| 21.4.4.41.3 | TimeZoneString ( tv ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-timezoneestring) |
| 21.4.4.41.4 | ToDateString ( tv ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-todatestring) |
| 21.4.4.42 | Date.prototype.toTimeString ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.totimestring) |
| 21.4.4.43 | Date.prototype.toUTCString ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.toutcstring) |
| 21.4.4.44 | Date.prototype.valueOf ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.valueof) |
| 21.4.4.45 | Date.prototype [ %Symbol.toPrimitive% ] ( hint ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype-%symbol.toprimitive%) |
| 21.4.5 | Properties of Date Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-date-instances) |

## Support

Feature-level support tracking with test script references.

### 21.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-overview-of-date-objects-and-definitions-of-abstract-operations))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| new Date() (current time) | Supported | `Js2IL.Tests/Date/ExecutionTests.cs` | Constructs a Date representing now (UTC). Stores milliseconds since Unix epoch internally. |
| new Date(milliseconds) | Supported | [`Date_Construct_FromMs_GetTime_ToISOString.js`](../../../Js2IL.Tests/Date/JavaScript/Date_Construct_FromMs_GetTime_ToISOString.js) | Constructs from milliseconds since Unix epoch; numeric argument is coerced per JS ToNumber minimal behavior. |

### 21.4.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-date))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Date.now() | Supported | `Js2IL.Tests/Date/ExecutionTests.cs` | Returns current time in milliseconds since Unix epoch as a number (boxed double). |

### 21.4.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-date.parse))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Date.parse(string) | Supported | `Js2IL.Tests/Date/ExecutionTests.cs`<br>[`Date_Parse_IsoString.js`](../../../Js2IL.Tests/Date/JavaScript/Date_Parse_IsoString.js) | Parses an ISO-like string to milliseconds since Unix epoch, or NaN on failure; returns a number (boxed double). |

### 21.4.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-date.prototype.gettime))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Date.prototype.getTime | Supported | [`Date_Construct_FromMs_GetTime_ToISOString.js`](../../../Js2IL.Tests/Date/JavaScript/Date_Construct_FromMs_GetTime_ToISOString.js) | Returns milliseconds since Unix epoch as a number (boxed double). |

### 21.4.3.27 ([tc39.es](https://tc39.es/ecma262/#sec-date.prototype.toisostring))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Date.prototype.toISOString | Supported | [`Date_Construct_FromMs_GetTime_ToISOString.js`](../../../Js2IL.Tests/Date/JavaScript/Date_Construct_FromMs_GetTime_ToISOString.js) | Returns a UTC ISO 8601 string with millisecond precision and trailing 'Z'. |

