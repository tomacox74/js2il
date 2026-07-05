<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 21.4: Date Objects

[Back to Section21](Section21.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-05T14:54:51Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 21.4 | Date Objects | Supported | [tc39.es](https://tc39.es/ecma262/#sec-date-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 21.4.1 | Overview of Date Objects and Definitions of Abstract Operations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-overview-of-date-objects-and-definitions-of-abstract-operations) |
| 21.4.1.1 | Time Values and Time Range | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-time-values-and-time-range) |
| 21.4.1.2 | Time-related Constants | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-time-related-constants) |
| 21.4.1.3 | Day ( t ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-day) |
| 21.4.1.4 | TimeWithinDay ( t ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-timewithinday) |
| 21.4.1.5 | DaysInYear ( y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-daysinyear) |
| 21.4.1.6 | DayFromYear ( y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-dayfromyear) |
| 21.4.1.7 | TimeFromYear ( y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-timefromyear) |
| 21.4.1.8 | YearFromTime ( t ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-yearfromtime) |
| 21.4.1.9 | DayWithinYear ( t ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-daywithinyear) |
| 21.4.1.10 | InLeapYear ( t ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-inleapyear) |
| 21.4.1.11 | MonthFromTime ( t ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-monthfromtime) |
| 21.4.1.12 | DateFromTime ( t ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-datefromtime) |
| 21.4.1.13 | WeekDay ( t ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weekday) |
| 21.4.1.14 | HourFromTime ( t ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-hourfromtime) |
| 21.4.1.15 | MinFromTime ( t ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-minfromtime) |
| 21.4.1.16 | SecFromTime ( t ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-secfromtime) |
| 21.4.1.17 | msFromTime ( t ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-msfromtime) |
| 21.4.1.18 | GetUTCEpochNanoseconds ( year , month , day , hour , minute , second , millisecond , microsecond , nanosecond ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getutcepochnanoseconds) |
| 21.4.1.19 | Time Zone Identifiers | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-time-zone-identifiers) |
| 21.4.1.20 | GetNamedTimeZoneEpochNanoseconds ( timeZoneIdentifier , year , month , day , hour , minute , second , millisecond , microsecond , nanosecond ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getnamedtimezoneepochnanoseconds) |
| 21.4.1.21 | GetNamedTimeZoneOffsetNanoseconds ( timeZoneIdentifier , epochNanoseconds ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getnamedtimezoneoffsetnanoseconds) |
| 21.4.1.22 | Time Zone Identifier Record | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-time-zone-identifier-record) |
| 21.4.1.23 | AvailableNamedTimeZoneIdentifiers ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-availablenamedtimezoneidentifiers) |
| 21.4.1.24 | SystemTimeZoneIdentifier ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-systemtimezoneidentifier) |
| 21.4.1.25 | LocalTime ( t ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-localtime) |
| 21.4.1.26 | UTC ( t ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-utc-t) |
| 21.4.1.27 | MakeTime ( hour , min , sec , ms ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-maketime) |
| 21.4.1.28 | MakeDay ( year , month , date ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-makeday) |
| 21.4.1.29 | MakeDate ( day , time ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-makedate) |
| 21.4.1.30 | MakeFullYear ( year ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-makefullyear) |
| 21.4.1.31 | TimeClip ( time ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-timeclip) |
| 21.4.1.32 | Date Time String Format | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date-time-string-format) |
| 21.4.1.32.1 | Expanded Years | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-expanded-years) |
| 21.4.1.33 | Time Zone Offset String Format | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-time-zone-offset-strings) |
| 21.4.1.33.1 | IsTimeZoneOffsetString ( offsetString ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-istimezoneoffsetstring) |
| 21.4.1.33.2 | ParseTimeZoneOffsetString ( offsetString ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-parsetimezoneoffsetstring) |
| 21.4.2 | The Date Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date-constructor) |
| 21.4.2.1 | Date ( ... values ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date) |
| 21.4.3 | Properties of the Date Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-date-constructor) |
| 21.4.3.1 | Date.now ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-date.now) |
| 21.4.3.2 | Date.parse ( string ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-date.parse) |
| 21.4.3.3 | Date.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype) |
| 21.4.3.4 | Date.UTC ( year [ , month [ , date [ , hours [ , minutes [ , seconds [ , ms ] ] ] ] ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.utc) |
| 21.4.4 | Properties of the Date Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-date-prototype-object) |
| 21.4.4.1 | Date.prototype.constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.constructor) |
| 21.4.4.2 | Date.prototype.getDate ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getdate) |
| 21.4.4.3 | Date.prototype.getDay ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getday) |
| 21.4.4.4 | Date.prototype.getFullYear ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getfullyear) |
| 21.4.4.5 | Date.prototype.getHours ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.gethours) |
| 21.4.4.6 | Date.prototype.getMilliseconds ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getmilliseconds) |
| 21.4.4.7 | Date.prototype.getMinutes ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getminutes) |
| 21.4.4.8 | Date.prototype.getMonth ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getmonth) |
| 21.4.4.9 | Date.prototype.getSeconds ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getseconds) |
| 21.4.4.10 | Date.prototype.getTime ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.gettime) |
| 21.4.4.11 | Date.prototype.getTimezoneOffset ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.gettimezoneoffset) |
| 21.4.4.12 | Date.prototype.getUTCDate ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutcdate) |
| 21.4.4.13 | Date.prototype.getUTCDay ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutcday) |
| 21.4.4.14 | Date.prototype.getUTCFullYear ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutcfullyear) |
| 21.4.4.15 | Date.prototype.getUTCHours ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutchours) |
| 21.4.4.16 | Date.prototype.getUTCMilliseconds ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutcmilliseconds) |
| 21.4.4.17 | Date.prototype.getUTCMinutes ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutcminutes) |
| 21.4.4.18 | Date.prototype.getUTCMonth ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutcmonth) |
| 21.4.4.19 | Date.prototype.getUTCSeconds ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getutcseconds) |
| 21.4.4.20 | Date.prototype.setDate ( date ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setdate) |
| 21.4.4.21 | Date.prototype.setFullYear ( year [ , month [ , date ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setfullyear) |
| 21.4.4.22 | Date.prototype.setHours ( hour [ , min [ , sec [ , ms ] ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.sethours) |
| 21.4.4.23 | Date.prototype.setMilliseconds ( ms ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setmilliseconds) |
| 21.4.4.24 | Date.prototype.setMinutes ( min [ , sec [ , ms ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setminutes) |
| 21.4.4.25 | Date.prototype.setMonth ( month [ , date ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setmonth) |
| 21.4.4.26 | Date.prototype.setSeconds ( sec [ , ms ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setseconds) |
| 21.4.4.27 | Date.prototype.setTime ( time ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.settime) |
| 21.4.4.28 | Date.prototype.setUTCDate ( date ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setutcdate) |
| 21.4.4.29 | Date.prototype.setUTCFullYear ( year [ , month [ , date ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setutcfullyear) |
| 21.4.4.30 | Date.prototype.setUTCHours ( hour [ , min [ , sec [ , ms ] ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setutchours) |
| 21.4.4.31 | Date.prototype.setUTCMilliseconds ( ms ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setutcmilliseconds) |
| 21.4.4.32 | Date.prototype.setUTCMinutes ( min [ , sec [ , ms ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setutcminutes) |
| 21.4.4.33 | Date.prototype.setUTCMonth ( month [ , date ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setutcmonth) |
| 21.4.4.34 | Date.prototype.setUTCSeconds ( sec [ , ms ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.setutcseconds) |
| 21.4.4.35 | Date.prototype.toDateString ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.todatestring) |
| 21.4.4.36 | Date.prototype.toISOString ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.toisostring) |
| 21.4.4.37 | Date.prototype.toJSON ( key ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.tojson) |
| 21.4.4.38 | Date.prototype.toLocaleDateString ( [ reserved1 [ , reserved2 ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.tolocaledatestring) |
| 21.4.4.39 | Date.prototype.toLocaleString ( [ reserved1 [ , reserved2 ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.tolocalestring) |
| 21.4.4.40 | Date.prototype.toLocaleTimeString ( [ reserved1 [ , reserved2 ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.tolocaletimestring) |
| 21.4.4.41 | Date.prototype.toString ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.tostring) |
| 21.4.4.41.1 | TimeString ( tv ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-timestring) |
| 21.4.4.41.2 | DateString ( tv ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-datestring) |
| 21.4.4.41.3 | TimeZoneString ( tv ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-timezoneestring) |
| 21.4.4.41.4 | ToDateString ( tv ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-todatestring) |
| 21.4.4.42 | Date.prototype.toTimeString ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.totimestring) |
| 21.4.4.43 | Date.prototype.toUTCString ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.toutcstring) |
| 21.4.4.44 | Date.prototype.valueOf ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype.valueof) |
| 21.4.4.45 | Date.prototype [ %Symbol.toPrimitive% ] ( hint ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-date.prototype-%symbol.toprimitive%) |
| 21.4.5 | Properties of Date Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-date-instances) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 21.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-overview-of-date-objects-and-definitions-of-abstract-operations))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Representative Section 21.4 abstract operation buckets | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Date/Section21_4/Clause_21_4_1/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Date/Section21_4/Clause_21_4_1_32/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Date/Section21_4/Clause_21_4_1_33/ExecutionTests.cs` |  | The repo now tracks every previously untracked 21.4.1 clause with representative checked-in test262 ports in the Section21_4 bucket tree. Coverage is intentionally representative rather than exhaustive, so these abstract-operation clauses remain documented as supported with limitations. |

### 21.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-date-constructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Date constructor basics (2-7 numeric arguments, Date copy, and TimeClip -0 normalization) | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Date/ExecutionTests.cs` | `test/built-ins/Date/construct_with_date.js`<br>`test/built-ins/Date/S15.9.3.1_A1_T1.js`<br>`test/built-ins/Date/S15.9.3.1_A1_T2.js`<br>`test/built-ins/Date/S15.9.3.1_A1_T3.js`<br>`test/built-ins/Date/S15.9.3.1_A1_T4.js`<br>`test/built-ins/Date/S15.9.3.1_A1_T5.js`<br>`test/built-ins/Date/S15.9.3.1_A1_T6.js`<br>`test/built-ins/Date/TimeClip_negative_zero.js` | Checked-in coverage now includes representative 2-7 argument Date construction, copying an existing Date instance without observable user coercion, and TimeClip normalization of -0 to +0. Broader Date parsing, time-zone, and prototype method semantics remain limited. |
| new Date() (current time) | Supported | `tests/Jroc.Tests/Date/ExecutionTests.cs` |  | Constructs a Date representing now (UTC). Stores milliseconds since Unix epoch internally. |
| new Date(milliseconds) | Supported | [`Date_Construct_FromMs_GetTime_ToISOString.js`](../../../tests/Jroc.Tests/Date/JavaScript/Date_Construct_FromMs_GetTime_ToISOString.js) | `test/built-ins/Date/prototype/valueOf/S9.4_A3_T1.js`<br>`test/built-ins/Date/prototype/getTime/this-value-valid-date.js` | Constructs from milliseconds since Unix epoch; numeric input is TimeClipped, invalid time values propagate as NaN, and Date instances satisfy instanceof Date. |

### 21.4.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-date.now))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Date.now() | Supported | `tests/Jroc.Test262.Tests/built-ins/Date/now/ExecutionTests.cs` | `test/built-ins/Date/now/15.9.4.4-0-4.js`<br>`test/built-ins/Date/now/name.js` | Returns current time in milliseconds since Unix epoch as a number (boxed double). |

### 21.4.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-date.parse))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Date.parse(string) | Supported | `tests/Jroc.Test262.Tests/built-ins/Date/parse/ExecutionTests.cs` | `test/built-ins/Date/parse/year-zero.js`<br>`test/built-ins/Date/parse/prop-desc.js`<br>`test/built-ins/Date/parse/not-a-constructor.js`<br>`test/built-ins/Date/parse/name.js`<br>`test/built-ins/Date/parse/length.js` | Parses ISO-like strings to milliseconds since Unix epoch, including year-only forms and the split between offsetless date-only (UTC) and offsetless date-time (local time); returns NaN on failure. |

### 21.4.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-date.utc))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Date.UTC | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Date/Section21_4/Clause_21_4_3_4/ExecutionTests.cs` | `test/built-ins/Date/UTC/prop-desc.js`<br>`test/built-ins/Date/UTC/length.js` | Date.UTC is exposed with checked-in descriptor and length coverage through the Section21_4 representative bucket. Broader argument-edge-case coverage is still limited. |

### 21.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-date-prototype-object))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Broader Date.prototype surface tracked by Section21_4 buckets | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Date/Section21_4/Clause_21_4_4_1/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Date/Section21_4/Clause_21_4_4_37/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Date/Section21_4/Clause_21_4_4_45/ExecutionTests.cs` |  | The Section21_4 representative buckets now cover the previously untracked Date.prototype constructor, local/UTC getters, setters, string/JSON conversions, and Symbol.toPrimitive metadata. Those APIs are now tracked in-repo, but many remain documented as supported with limitations because the current coverage is representative rather than full conformance. |

### 21.4.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getfullyear))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Date.prototype.getFullYear | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Date/prototype/getFullYear/ExecutionTests.cs` | `test/built-ins/Date/prototype/getFullYear/prop-desc.js`<br>`test/built-ins/Date/prototype/getFullYear/this-value-valid-date.js` | Implemented as a Date instance method returning the local calendar year from the internal millisecond timestamp, with representative checked-in coverage for boundary rollovers. Broader locale/time-zone edge cases remain limited. |

### 21.4.4.8 ([tc39.es](https://tc39.es/ecma262/#sec-date.prototype.getmonth))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Date.prototype.getMonth | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Date/prototype/getMonth/ExecutionTests.cs` | `test/built-ins/Date/prototype/getMonth/prop-desc.js`<br>`test/built-ins/Date/prototype/getMonth/this-value-valid-date.js` | Implemented as a Date instance method returning the local zero-based month from the internal millisecond timestamp, with representative checked-in coverage for boundary rollovers. Broader locale/time-zone edge cases remain limited. |

### 21.4.4.10 ([tc39.es](https://tc39.es/ecma262/#sec-date.prototype.gettime))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Date.prototype.getTime | Supported | `tests/Jroc.Test262.Tests/built-ins/Date/prototype/getTime/ExecutionTests.cs` | `test/built-ins/Date/prototype/getTime/this-value-valid-date.js`<br>`test/built-ins/Date/prototype/getTime/length.js` | Returns milliseconds since Unix epoch as a number (boxed double). |

### 21.4.4.36 ([tc39.es](https://tc39.es/ecma262/#sec-date.prototype.toisostring))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Date.prototype.toISOString | Supported | `tests/Jroc.Test262.Tests/built-ins/Date/prototype/toISOString/ExecutionTests.cs` | `test/built-ins/Date/prototype/toISOString/15.9.5.43-0-16.js`<br>`test/built-ins/Date/prototype/toISOString/15.9.5.43-0-5.js`<br>`test/built-ins/Date/prototype/toISOString/15.9.5.43-0-6.js`<br>`test/built-ins/Date/prototype/toISOString/15.9.5.43-0-7.js` | Returns a UTC ISO 8601 string with millisecond precision and trailing 'Z'. |

