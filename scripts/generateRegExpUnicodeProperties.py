#!/usr/bin/env python3
"""Generate Unicode properties, case mappings, and emoji strings from pinned official data."""

import argparse
from collections import defaultdict
import hashlib
import urllib.request

from generateRegExpUnicodeScripts import (
    ROOT, VERSION, case_fold_additions, case_folding_groups, code_point_range,
    merge, read_ucd, records, subtract,
)


OUTPUT = ROOT / "src/JavaScriptRuntime/RegExpUnicodeProperties.Generated.cs"
BINARY_PROPERTIES = (
    "ASCII", "ASCII_Hex_Digit", "Alphabetic", "Any", "Assigned",
    "Bidi_Control", "Bidi_Mirrored", "Case_Ignorable", "Cased",
    "Changes_When_Casefolded", "Changes_When_Casemapped", "Changes_When_Lowercased",
    "Changes_When_NFKC_Casefolded", "Changes_When_Titlecased", "Changes_When_Uppercased",
    "Dash", "Default_Ignorable_Code_Point", "Deprecated", "Diacritic",
    "Emoji", "Emoji_Component", "Emoji_Modifier", "Emoji_Modifier_Base",
    "Emoji_Presentation", "Extended_Pictographic", "Extender", "Grapheme_Base",
    "Grapheme_Extend", "Hex_Digit", "IDS_Binary_Operator", "IDS_Trinary_Operator",
    "ID_Continue", "ID_Start", "Ideographic", "Join_Control", "Logical_Order_Exception",
    "Lowercase", "Math", "Noncharacter_Code_Point", "Pattern_Syntax", "Pattern_White_Space",
    "Quotation_Mark", "Radical", "Regional_Indicator", "Sentence_Terminal", "Soft_Dotted",
    "Terminal_Punctuation", "Unified_Ideograph", "Uppercase", "Variation_Selector",
    "White_Space", "XID_Continue", "XID_Start",
)
EMOJI_SOURCES = {
    "emoji-sequences.txt": "12cc8267dc33cbd11ed32bcf6fc5dc2ad9c7a77bae1bdfba2f41b1b9b3ead8dd",
    "emoji-zwj-sequences.txt": "5b25441daed2322b068c5e70cda522946a4f0274df864445a1965a92e5fc5cad",
}


def emoji_properties():
    properties = defaultdict(set)
    for name, digest in EMOJI_SOURCES.items():
        path = ROOT / "artifacts/unicode" / VERSION / name
        if not path.exists():
            path.parent.mkdir(parents=True, exist_ok=True)
            with urllib.request.urlopen(f"https://www.unicode.org/Public/{VERSION}/emoji/{name}", timeout=60) as response:
                path.write_bytes(response.read())
        data = path.read_bytes()
        if hashlib.sha256(data).hexdigest() != digest:
            raise SystemExit(f"Unicode emoji source checksum mismatch: {name}")
        for fields in records(data.decode("utf-8")):
            value, property = fields[:2]
            if ".." in value:
                first, last = code_point_range(value)
                properties[property].update(chr(point) for point in range(first, last + 1))
            else:
                properties[property].add("".join(chr(int(point, 16)) for point in value.split()))
    properties["RGI_Emoji"] = set().union(*properties.values())
    return properties


def escape_string(value):
    return "".join(f"\\u{int.from_bytes(value[index:index + 2], 'big'):04X}"
                   for index in range(0, len(value), 2))


def generate():
    data = read_ucd((
        "extracted/DerivedGeneralCategory.txt", "extracted/DerivedBinaryProperties.txt",
        "DerivedCoreProperties.txt", "PropList.txt", "PropertyAliases.txt",
        "PropertyValueAliases.txt", "CaseFolding.txt", "DerivedNormalizationProps.txt",
        "emoji/emoji-data.txt",
        "UnicodeData.txt", "SpecialCasing.txt",
    ))
    categories = defaultdict(list)
    for value, category in records(data["extracted/DerivedGeneralCategory.txt"]):
        categories[category].append(code_point_range(value))
    categories = {name: merge(ranges) for name, ranges in categories.items()}
    properties = {}
    category_aliases = {}
    for fields in records(data["PropertyValueAliases.txt"]):
        if fields[0] != "gc":
            continue
        short, name = fields[1:3]
        members = ("Lu", "Ll", "Lt") if short == "LC" else (
            [key for key in categories if key.startswith(short)] if len(short) == 1 else [short])
        properties[name] = merge(interval for member in members for interval in categories[member])
        category_aliases[name] = fields[1:]

    binary = {name: [] for name in BINARY_PROPERTIES}
    binary_aliases = {name: [name] for name in binary}
    for fields in records(data["PropertyAliases.txt"]):
        if fields[1] in binary:
            binary_aliases[fields[1]] = fields
    for file in ("DerivedCoreProperties.txt", "PropList.txt", "extracted/DerivedBinaryProperties.txt",
                 "DerivedNormalizationProps.txt", "emoji/emoji-data.txt"):
        for fields in records(data[file]):
            if fields[1] in binary:
                binary[fields[1]].append(code_point_range(fields[0]))
    binary["ASCII"] = [(0, 127)]
    binary["Any"] = [(0, 0x10FFFF)]
    binary["Assigned"] = subtract([(0, 0x10FFFF)], categories["Cn"])
    assert all(binary.values()), "Missing UCD binary property"
    properties.update({name: merge(ranges) for name, ranges in binary.items()})
    ordered = sorted(category_aliases) + sorted(binary_aliases)
    groups = case_folding_groups(data["CaseFolding.txt"])

    lines = [
        "// <auto-generated />",
        f"// Unicode {VERSION}; generated by scripts/generateRegExpUnicodeProperties.py.",
        "// Copyright (c) 2025 Unicode, Inc. Licensed under Unicode-3.0.",
        "// See Unicode-LICENSE.txt for the copyright and permission notice.",
        "#nullable enable",
        "",
        "namespace JavaScriptRuntime;",
        "",
        "internal static class RegExpUnicodeProperties",
        "{",
    ]
    for method, aliases in (("ResolveGeneralCategory", category_aliases), ("ResolveBinaryProperty", binary_aliases)):
        lines.extend([f"    internal static int {method}(string value) => value switch", "    {"])
        for name, values in sorted(aliases.items()):
            labels = " or ".join(f'"{alias}"' for alias in sorted(set(values)))
            lines.append(f"        {labels} => {ordered.index(name)},")
        lines.extend(["        _ => -1", "    };", ""])

    lines.extend([
        "    internal static ReadOnlySpan<int> GetRanges(int property) => property switch",
        "    {",
    ])
    for index, name in enumerate(ordered):
        flat = [f"0x{point:X}" for interval in properties[name] for point in interval]
        lines.extend([f"        {index} =>", "        ["])
        for start in range(0, len(flat), 10):
            lines.append("            " + ", ".join(flat[start:start + 10]) + ",")
        lines.extend(["        ],"])
    lines.extend([
        "        _ => throw new ArgumentOutOfRangeException(nameof(property))",
        "    };",
        "",
        "    internal static ReadOnlySpan<int> GetCaseFoldAdditions(int property, bool complement)",
        "        => (property, complement) switch",
        "    {",
    ])
    for index, name in enumerate(ordered):
        for complement in (False, True):
            additions = case_fold_additions(properties[name], complement, groups)
            if additions:
                lines.extend([f"        ({index}, {str(complement).lower()}) =>", "        ["])
                values = [f"0x{point:X}" for point in additions]
                for start in range(0, len(values), 10):
                    lines.append("            " + ", ".join(values[start:start + 10]) + ",")
                lines.append("        ],")
    lines.extend(["        _ => []", "    };", ""])
    lines.extend([
        "    internal static ReadOnlySpan<int> GetSimpleCaseFoldMappings() =>",
        "    [",
    ])
    mappings = sorted(
        (int(point, 16), int(mapping, 16))
        for point, status, mapping, *_ in records(data["CaseFolding.txt"])
        if status in ("C", "S"))
    for start in range(0, len(mappings), 5):
        values = [f"0x{point:X}, 0x{target:X}" for point, target in mappings[start:start + 5]]
        lines.append("        " + ", ".join(values) + ",")
    lines.extend(["    ];", "", "    internal static ReadOnlySpan<int> GetLegacyCaseFoldMappings() =>", "    ["])
    expanded = {
        int(fields[0], 16)
        for fields in records(data["SpecialCasing.txt"])
        if not fields[4] and len(fields[3].split()) != 1
    }
    legacy = []
    for fields in records(data["UnicodeData.txt"]):
        point = int(fields[0], 16)
        if point > 0xFFFF or point in expanded or not fields[12]:
            continue
        target = int(fields[12], 16)
        if target <= 0xFFFF and not (point >= 128 and target < 128) and target != point:
            legacy.append((point, target))
    for start in range(0, len(legacy), 5):
        lines.append("        " + ", ".join(
            f"0x{point:X}, 0x{target:X}" for point, target in legacy[start:start + 5]) + ",")
    lines.extend(["    ];", "", "    internal static string? GetStrings(string property) => property switch", "    {"])
    for name, strings in sorted(emoji_properties().items()):
        lines.append(f'        "{name}" =>')
        for index, string in enumerate(sorted(strings)):
            encoded = escape_string(string.encode("utf-16-be"))
            if index:
                encoded = "\\n" + encoded
            lines.append(f'            "{encoded}"' + (" +" if index + 1 < len(strings) else ","))
    lines.extend(["        _ => null", "    };", "}", ""])
    return "\n".join(lines)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    generated = generate()
    if args.check:
        if not OUTPUT.exists() or OUTPUT.read_text(encoding="utf-8") != generated:
            raise SystemExit(f"Generated Unicode property tables are out of date: {OUTPUT}")
        print(f"Verified {OUTPUT.relative_to(ROOT)}")
    else:
        OUTPUT.write_text(generated, encoding="utf-8", newline="\n")
        print(f"Generated {OUTPUT.relative_to(ROOT)}")


if __name__ == "__main__":
    main()
