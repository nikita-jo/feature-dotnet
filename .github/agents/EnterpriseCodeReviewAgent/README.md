# Enterprise Code Review Agent

Reviews the entire source tree together with the coverage, SonarQube and CodeQL artefacts and
produces a single executive-grade markdown report.

## Inputs
- Source tree (`src/`)
- Test tree (`tests/`)
- `coverage-summary.json`
- `security-summary.json`
- `security-analysis.json`

## Outputs
- `code-review-report.md`
- `code-review-report.json`

## Score Dimensions
- Architecture
- Performance
- Security
- Maintainability
- Technical Debt
- Best Practices

The agent also computes an overall score (avg of dimensions, scaled 0-10) and the final rating.
