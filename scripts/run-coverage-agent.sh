#!/usr/bin/env bash
# scripts/run-coverage-agent.sh
# Local invocation of the Coverage Analysis Agent (Node 18+).
set -euo pipefail
COVERAGE_FILE="${1:-coverage.cobertura.xml}"
THRESHOLD="${2:-85}"

if ! command -v node >/dev/null; then
  echo "Node.js is required."
  exit 1
fi

node .github/agents/CoverageAnalysisAgent/coverage-analysis-agent.mjs "$COVERAGE_FILE" "$THRESHOLD"