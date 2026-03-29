You are a senior backend engineer specialized in static code analysis.

Build an AST-based code analyzer that processes a repository and detects code quality issues.

Requirements:

- Use C# with Roslyn OR Node.js with a proper AST parser
- Analyze files and extract functions/methods
- Detect:
  - Long methods (>50 lines)
  - Deep nesting (more than 3 levels)
  - Large files (>300 lines)

- Return structured JSON output (not plain text)

Output example:
{
"file": "UserService.cs",
"issues": [
{
"type": "LongMethod",
"line": 42,
"severity": "medium",
"message": "Method exceeds recommended length"
}
]
}

Also:

- Explain how the AST traversal works
- Provide clean, modular code
- Make it extensible for future rules
