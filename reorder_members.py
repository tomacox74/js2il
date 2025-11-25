#!/usr/bin/env python3
"""
Reorder class members in ILExpressionGenerator.cs to conform to C# coding guidelines.
Order: Fields → Constructor → Public Methods → Internal Methods → Private Methods
"""

import re

def find_method_boundaries(lines):
    """Find start and end lines of each method."""
    methods = []
    i = 0
    while i < len(lines):
        line = lines[i].strip()
        
        # Match method declarations
        method_match = re.match(r'^\s*(public|private|internal|protected)(\s+static)?\s+.*\s+(\w+)\s*\(', lines[i])
        
        if method_match:
            access = method_match.group(1)
            is_static = method_match.group(2) is not None
            method_name = method_match.group(3)
            start = i
            
            # Find the end of this method by counting braces
            brace_count = 0
            in_method = False
            
            for j in range(i, len(lines)):
                for char in lines[j]:
                    if char == '{':
                        brace_count += 1
                        in_method = True
                    elif char == '}':
                        brace_count -= 1
                
                if in_method and brace_count == 0:
                    methods.append({
                        'access': access,
                        'is_static': is_static,
                        'name': method_name,
                        'start': start,
                        'end': j + 1,
                        'lines': lines[start:j+1]
                    })
                    i = j + 1
                    break
            else:
                i += 1
        else:
            i += 1
    
    return methods

def reorder_file():
    input_file = r'c:\git\js2il\Js2IL\Services\ILGenerators\ILExpressionGenerator.cs'
    
    with open(input_file, 'r', encoding='utf-8') as f:
        lines = f.readlines()
    
    # Find class start (after constructor ends)
    constructor_end = None
    for i, line in enumerate(lines):
        if 'public ILExpressionGenerator(ILMethodGenerator owner)' in line:
            # Find end of constructor
            brace_count = 0
            for j in range(i, len(lines)):
                for char in lines[j]:
                    if char == '{':
                        brace_count += 1
                    elif char == '}':
                        brace_count -= 1
                if brace_count == 0 and '{' in ''.join(lines[i:j+1]):
                    constructor_end = j + 1
                    break
            break
    
    if not constructor_end:
        print("Could not find constructor end")
        return
    
    # Extract header (usings, namespace, class declaration, fields, constructor)
    header = lines[:constructor_end]
    
    # Find closing brace of class
    class_end = len(lines) - 1
    for i in range(len(lines) - 1, -1, -1):
        if lines[i].strip() == '}' and i > constructor_end:
            # Check if this is the class closing brace (should have namespace closing after)
            if i < len(lines) - 1 and lines[i+1].strip() == '}':
                class_end = i
                break
    
    footer = lines[class_end:]
    
    # Extract methods from the body
    body_lines = lines[constructor_end:class_end]
    methods = find_method_boundaries(body_lines)
    
    # Categorize methods
    public_methods = [m for m in methods if m['access'] == 'public']
    internal_methods = [m for m in methods if m['access'] == 'internal']
    private_methods = [m for m in methods if m['access'] == 'private']
    
    print(f"Found {len(public_methods)} public, {len(internal_methods)} internal, {len(private_methods)} private methods")
    
    # Reconstruct file in correct order
    output_lines = header.copy()
    
    # Add public methods
    output_lines.append('\n')
    for method in public_methods:
        output_lines.extend(method['lines'])
        output_lines.append('\n')
    
    # Add internal methods
    for method in internal_methods:
        output_lines.extend(method['lines'])
        output_lines.append('\n')
    
    # Add private methods
    for method in private_methods:
        output_lines.extend(method['lines'])
        output_lines.append('\n')
    
    output_lines.extend(footer)
    
    # Write output
    with open(input_file, 'w', encoding='utf-8') as f:
        f.writelines(output_lines)
    
    print("File reordered successfully")

if __name__ == '__main__':
    reorder_file()
