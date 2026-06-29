# Sonar Security Agent

Queries the SonarQube Quality Gate API for the project, summarises the security posture and
fails the pipeline when the gate is not `OK`.

## Inputs
- Sonar host URL and token (from GitHub secrets).

## Outputs
- `security-summary.md`
- `security-summary.json`

## Behaviour
1. Calls `GET /api/qualitygates/project_status?projectKey=EmployeeManagementPortal`.
2. Maps each condition (bugs, vulnerabilities, code smells, coverage, duplications) into a table.
3. Fails the pipeline if `qualityGate != OK`.
