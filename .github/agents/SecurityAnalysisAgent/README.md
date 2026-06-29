# Security Analysis Agent

Consumes CodeQL SARIF output, counts findings by severity, maps them to OWASP Top 10 / CWE and
emits a security-analysis report.

## Inputs
- `*.sarif` files produced by CodeQL.

## Outputs
- `security-analysis.md`
- `security-analysis.json`
